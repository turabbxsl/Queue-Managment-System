using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Enums;

namespace QMS.Infrastructure.Context
{
    public class QMSDbContext : DbContext
    {
        public QMSDbContext(DbContextOptions<QMSDbContext> options) : base(options)
        {

        }

        public DbSet<Desk> Desks => Set<Desk>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<CustomerType>();
            modelBuilder.HasPostgresEnum<ServiceType>();
            modelBuilder.HasPostgresEnum<TicketStatus>();
            modelBuilder.HasPostgresEnum<DeskType>();

            modelBuilder.Entity<Desk>(entity =>
            {
                entity.ToTable("desks");
                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.DeskType).HasColumnName("desk_type");
                entity.Property(x => x.DeskName).HasColumnName("desk_name");
                entity.Property(x => x.IsBusy).HasColumnName("is_busy");
                entity.Property(x => x.HeadTicketId).HasColumnName("head_ticket_id");
                entity.Property(x => x.TailTicketId).HasColumnName("tail_ticket_id");
                entity.Property(x => x.QueueCount).HasColumnName("queue_count");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("tickets");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.TicketNumber).HasColumnName("ticket_number");
                entity.Property(x => x.CustomerFullName).HasColumnName("customer_full_name");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");

                entity.Property(x => x.ServiceType).HasColumnName("service_type");
                entity.Property(x => x.CustomerType).HasColumnName("customer_type");
                entity.Property(x => x.Status).HasColumnName("status");

                entity.Property(x => x.NextTicketId).HasColumnName("next_ticket_id");
                entity.Property(x => x.PreviousTicketId).HasColumnName("previous_ticket_id");
                entity.Property(x => x.DeskId).HasColumnName("desk_id");

                entity.HasOne(x => x.Desk)
                  .WithMany(d => d.Tickets)
                  .HasForeignKey(x => x.DeskId);

                entity.HasOne(x => x.NextTicket)
                      .WithOne()
                      .HasForeignKey<Ticket>(x => x.NextTicketId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.PreviousTicket)
                      .WithOne()
                      .HasForeignKey<Ticket>(x => x.PreviousTicketId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            base.OnModelCreating(modelBuilder);
        }



    }
}
