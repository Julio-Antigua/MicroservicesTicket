using AutoMapper;
using Common.Core.Events;
using static Ticketing.Command.Feature.Tickets.TicketCreate;

namespace Ticketing.Command.Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TicketCreateRequest, TicketCreatedEvent>(); 
        }
    }
}
