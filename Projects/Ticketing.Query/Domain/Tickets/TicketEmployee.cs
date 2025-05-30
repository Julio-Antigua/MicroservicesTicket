using Ticketing.Query.Domain.Employees;

namespace Ticketing.Query.Domain.Tickets
{
    public class TicketEmployee
    {
        public virtual Ticket? Ticket { get; set; }
        public virtual Employee? Employee { get; set; }
        public Guid TicketId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
