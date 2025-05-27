using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
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
                var id = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
                var command = new TicketCreateCommand(id,request);
                var result = await mediator.Send(command, cancellantionToken);
                return Results.Ok(result);
            });
        }

        public sealed class TicketCreateRequest(string username, int typeError, string detailError)
        {
            public string Username { get; set; } = username;
            public int TypeError { get; set; } = typeError;
            public string DetailError { get; set; } = detailError;
        }

        public record TicketCreateCommand(string Id,TicketCreateRequest ticketCreateRequest) : IRequest<bool>;

        public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
        {
            public TicketCreateCommandValidator()
            {
                RuleFor(x => x.ticketCreateRequest)
                    .SetValidator(new TicketCreateValidator());

                RuleFor(x => x.Id).NotEmpty().WithMessage("Ingrese el id del evento");
            }
        }

        public class TicketCreateValidator : AbstractValidator<TicketCreateRequest> {
            public TicketCreateValidator()
            {
                RuleFor(x => x.Username)
                    .NotEmpty()
                        .WithMessage("Ingrese un username")
                    .EmailAddress()
                        .WithMessage("Debe ser un email");

                RuleFor(x => x.TypeError)
                    .NotEmpty()
                        .WithMessage("Debe existir el tipo de error")
                    .InclusiveBetween(1, 5)
                        .WithMessage("El rango debe ser de 1 a 5");

                RuleFor(x => x.DetailError).NotEmpty().WithMessage("Ingrese el detalle del error");
            }
        }

        public sealed class TicketCreateCommandHandler(
            IEventSourcingHandler<TicketAggregate> eventSourcingHandler
        ) : IRequestHandler<TicketCreateCommand, bool>
        {
            
            private readonly IEventSourcingHandler<TicketAggregate> _eventSourcingHandler = eventSourcingHandler;
            public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
            {
                var aggregate = new TicketAggregate(request); 
                await _eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
                return true;
            }
        }

    }
}
