using System.Reflection;

namespace Ticketing.Command.Feature.Apis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMinimalApis(this IServiceCollection services) 
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            var minimalApis = currentAssembly.GetTypes()
                .Where(t => typeof(IMinimalApi).IsAssignableFrom(t) 
                && t != typeof(IMinimalApi) && t.IsPublic && !t.IsAbstract );

            foreach (var minimalApi in minimalApis) 
            {
                services.AddSingleton(typeof(IMinimalApi), minimalApi);
            }

            return services;
        }
    }
}
