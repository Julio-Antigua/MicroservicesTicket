using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infrastructure.Configurations
{
    public class TicketEmployeeConfiguration : IEntityTypeConfiguration<TicketEmployee>
    {
        public void Configure(EntityTypeBuilder<TicketEmployee> builder)
        {
            builder.ToTable("ticket_employees");
        }
    }

    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");
            builder.HasKey(t => t.Id);

            builder.Property(x => x.TicketType)
                .HasConversion(
                ticketType => ticketType!.Id,
                value => TicketType.Create(value)
            );

            builder
                .HasMany(x => x.Employees)
                .WithMany(x => x.Tickets)
                .UsingEntity<TicketEmployee>(
                    e => e
                        .HasOne(x => x.Employee)
                        .WithMany(p => p.TicketEmployees)
                        .HasForeignKey(p => p.EmployeeId),
                    t => t
                        .HasOne(x => x.Ticket)
                        .WithMany(p => p.TicketEmployees)
                        .HasForeignKey(p => p.TicketId),
                    te =>
                    {
                        te.HasKey(t => new { t.TicketId, t.EmployeeId });
                    }
                );
        }
    }
}
