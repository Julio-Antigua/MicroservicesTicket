using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Feature.Apis;

namespace Ticketing.Command.Feature.Tickets
{
    public sealed class TicketCreate : IMinimalApi
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPost("/api/ticket", async (
                TicketCreateRequest request,
                IMediator mediator,
                CancellationToken cancellantionToken
            ) =>
            {
                var command = new TicketCreateCommand(request);
                var result = await mediator.Send(command, cancellantionToken);
                return Results.Ok(result);
            });
        }

        public sealed class TicketCreateRequest(string username, string typeError, string detailError)
        {
            public string Username { get; set; } = username;
            public string TypeError { get; set; } = typeError;
            public string DetailError { get; set; } = detailError;
        }

        public record TicketCreateCommand(TicketCreateRequest ticketCreateRequest) : IRequest<bool>;

        public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
        {
            public TicketCreateCommandValidator()
            {
                RuleFor(x => x.ticketCreateRequest)
                    .SetValidator(new TicketCreateValidator());
            }
        }

        public class TicketCreateValidator : AbstractValidator<TicketCreateRequest> {
            public TicketCreateValidator()
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("Ingrese un username");
                RuleFor(x => x.DetailError).NotEmpty().WithMessage("Ingrese el detalle del error");
            }
        }

        public sealed class TicketCreateCommandHandler(
            IEventModelRepository eventModelRepository,
            IMapper mapper
        ) : IRequestHandler<TicketCreateCommand, bool>
        {
            private readonly IEventModelRepository _eventModelRepository = eventModelRepository;
            private readonly IMapper _mapper = mapper;

            public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
            {
                var ticketEventData = _mapper.Map<TicketCreatedEvent>(request.ticketCreateRequest);

                var eventModel = new EventModel
                {
                    Timestamp = DateTime.UtcNow,
                    AggregateIdentifier = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString(),
                    AggregateType = "TicketAggregate",
                    Version = 1,
                    EventType = "TicketCreatedEvent",
                    EventData = ticketEventData
                };

                IClientSessionHandle session = await _eventModelRepository.BeginSessionAsync(cancellationToken);

                try
                {
                    _eventModelRepository.BeginTransaction(session);
                    await _eventModelRepository.InsertOneAsync(eventModel,session,cancellationToken);
                    await _eventModelRepository.CommitTransactionAsync(session,cancellationToken);
                    _eventModelRepository.DisposeSession(session);
                    return true;
                }
                catch (Exception ex) 
                {
                    await _eventModelRepository.RollebackTransactionAsync(session,cancellationToken);

                    _eventModelRepository.DisposeSession(session);

                    return false;
                }
            }
        }

    }
}
