using Common.Core.Events;
using Common.Core.Producers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Infrastructure.EventSourcing;
using Ticketing.Command.Infrastructure.Persistence;
using Ticketing.Command.Infrastructure.Repositories;

namespace Ticketing.Command.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration) 
        {
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<TicketCreatedEvent>();

            //Mantiene la mista instacia u objeto en todo el ciclo de vida
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddScoped<IEventProducer,TicketEventProducer>();

            //En cada instancia o componente class que consuma el IEventRepository se genere ua nueva instancia de este servicio
            services.AddTransient<IEventModelRepository, EventModelRepository>();

            services.AddTransient<IEventStore, EventStore>();
            services.AddTransient<IEventSourcingHandler<TicketAggregate>, TicketingEventSourcingHandler>();

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("MongoDb"))
            );

            return services;
        }
    }
}
