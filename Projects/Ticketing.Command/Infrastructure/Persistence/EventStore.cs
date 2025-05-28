using Common.Core.Events;
using Common.Core.Producers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Persistence
{
    public class EventStore : IEventStore
    {
        private readonly IEventModelRepository _eventModelRepository;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IEventProducer _eventProducer;

        public EventStore(
            IEventModelRepository eventModelRepository,
            IOptions<KafkaSettings> kafkaSettings,
            IEventProducer eventProducer)
        {
            _eventModelRepository = eventModelRepository;
            _kafkaSettings = kafkaSettings.Value;
            _eventProducer = eventProducer;
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

                //Envia mia eventos a la base de datos de mongodb
                await AddEventToken(eventModel, cancellationToken);

                //Envia el evento hacia al broker apache kafka
                var topic = _kafkaSettings.Topic ?? throw new Exception("No encontro el topic");
                await _eventProducer.ProduceAsync(topic,@event);
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
