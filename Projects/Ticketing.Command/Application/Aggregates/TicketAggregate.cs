using Common.Core.Events;
using System.ComponentModel;
using System.ComponentModel.Design;
using Ticketing.Command.Domain.Abstracts;
using static Ticketing.Command.Feature.Tickets.TicketCreate;

namespace Ticketing.Command.Application.Aggregates
{
    public class TicketAggregate : AggregateRoot
    {
        public bool Active { get; set; }

        public TicketAggregate() { }

        public TicketAggregate(TicketCreateCommand command)
        {
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = command.Id,
                Username = command.ticketCreateRequest.Username, 
                TypeError = command.ticketCreateRequest.TypeError,
                DetailError = command.ticketCreateRequest.DetailError,
            };

            RaiseEvent(ticketCreatedEvent);
        }

        public void Apply(TicketCreatedEvent @event)
        {
            _id = @event.Id;
            Active = true;
        }
    }
}
