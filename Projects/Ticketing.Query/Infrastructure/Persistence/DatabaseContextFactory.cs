using Microsoft.EntityFrameworkCore;

namespace Ticketing.Query.Infrastructure.Persistence
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public TicketDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<TicketDbContext> optionsBuilder = new();
            _configureDbContext(optionsBuilder);
            return new TicketDbContext(optionsBuilder.Options); //opciones de la cadena de conexion
        }
    }
}
