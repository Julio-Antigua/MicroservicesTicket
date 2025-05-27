using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;

namespace Ticketing.Command.Infrastructure.EventSourcing
{
    public class TicketingEventSourcingHandler : IEventSourcingHandler<TicketAggregate>
    {
        private readonly IEventStore _eventStore;

        public TicketingEventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<TicketAggregate> GetByIdAsync(string aggregateId, CancellationToken cancellationToken)
        {
            var aggregate = new TicketAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId, cancellationToken);
            if (events == null || !events.Any()) return aggregate;
            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();
            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            //GetUnconmmittedChanges() trae la lista de eventos que no estan la base de datos sino en memoria
            await _eventStore.SaveEventAsync(aggregateRoot.Id,aggregateRoot.GetUnconmmittedChanges(),aggregateRoot.Version, cancellationToken);

            //Esto hace que todos los eventos que en este momento que estan en memoria han sido insertados exitosamente en la base de datos
            aggregateRoot.MarkChangesAsCommited();
        }
    }
}
