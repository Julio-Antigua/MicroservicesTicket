using Common.Core.Events;
using MongoDB.Driver;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Persistence
{
    public class EventStore : IEventStore
    {
        private readonly IEventModelRepository _eventModelRepository;

        public EventStore(IEventModelRepository eventModelRepository)
        {
            _eventModelRepository = eventModelRepository;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken)
        {
            var eventStream = await _eventModelRepository.FilterByAsync(doc => doc.AggregateIdentifier == aggregateId,cancellationToken);
            if (eventStream is null || !eventStream.Any()) { throw new Exception("El aggregate no tiene eventos"); };
            return eventStream.OrderBy(e => e.Version).Select(x => x.EventData).ToList()!;
        }

        public async Task SaveEventAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken cancellationToken)
        {
            var eventStream = await _eventModelRepository.FilterByAsync(doc => doc.AggregateIdentifier==aggregateId,cancellationToken);
            if (eventStream.Any() && expectedVersion != -1 && eventStream.Last().Version != expectedVersion)
            {
                throw new Exception("Error de concurrencia");
            }
            var version = expectedVersion;
            foreach(var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel 
                {
                    Timestamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId, 
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };
                await AddEventToken(eventModel, cancellationToken);
            }
        }

        private async Task AddEventToken(EventModel eventModel, CancellationToken cancellationToken)
        {
            IClientSessionHandle session = await _eventModelRepository.BeginSessionAsync(cancellationToken);
            try
            {
                _eventModelRepository.BeginTransaction(session);
                await _eventModelRepository.InsertOneAsync(eventModel, session, cancellationToken);
                await _eventModelRepository.CommitTransactionAsync(session, cancellationToken);
                _eventModelRepository.DisposeSession(session);
            }
            catch (Exception ex)
            {
                await _eventModelRepository.RollebackTransactionAsync(session, cancellationToken);
                _eventModelRepository.DisposeSession(session);
            }
        }
    }
}
