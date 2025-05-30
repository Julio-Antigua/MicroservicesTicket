using Common.Core.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Ticketing.Query.Domain.TicketTypes
{
    public class TicketType
    {
        private TicketType()
        {
        }
        [SetsRequiredMembers]
        private TicketType(int id, string name) => (Id, Name) = (id, name);

        public int Id { get; set; }
        public required string Name { get; set; }

        public static TicketType Create(int id)
        {
            var ticketTypeEnum = (TicketTypeEnum)id;
            string stringValue = ticketTypeEnum.ToString();
            return new TicketType(id, stringValue);
        }
    }
}
