using Common.Core.Events;

namespace Ticketing.Command.Domain.Abstracts
{
    public abstract class AggregateRoot
    {
        protected string _id = string.Empty;
        public string Id { get { return _id; } }
        public int Version { get; set; }
        private readonly List<BaseEvent> _changes = new();

        public IEnumerable<BaseEvent> GetUnconmmittedChanges() { return _changes; }
        public void MarkChangesAsCommited()
        {
            _changes.Clear();
        }

        public void ApplyChange(BaseEvent @event, bool isNewEvent)
        {
            var method = GetType().GetMethod("Apply", [@event.GetType()]);
            if (method is null)
            {
                throw new ArgumentNullException(
                    nameof(method),
                    $"El motod Apply no fue encontrado dentor de {@event.GetType().Name}");
            }
            method.Invoke(this, [@event]);
            if (isNewEvent) { _changes.Add(@event); }
        }

        public void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event,true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events){
            foreach(var @event in events)
            {
                ApplyChange(@event,false);
            }
        }
    }
}
