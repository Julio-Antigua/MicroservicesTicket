using Common.Core.Events;

namespace Ticketing.Command.Domain.Abstracts
{
    public interface IEventStore
    {
        Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken);
        Task SaveEventAsync(
            string aggregateId,
            IEnumerable<BaseEvent> events,
            int expectdVersion,
            CancellationToken cancellationToken);
    }
}
