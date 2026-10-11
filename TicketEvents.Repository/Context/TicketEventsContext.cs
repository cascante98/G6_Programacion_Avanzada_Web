using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using TicketEvents.Entities.Models;

namespace TicketEvents.Repository.Context
{
    public class TicketEventsContext : IdentityDbContext<Usuario>
    {
        public TicketEventsContext()
            : base("TicketEventsConnection")
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<DetalleOrden> DetallesOrden { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public static TicketEventsContext Create()
        {
            return new TicketEventsContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Evento>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken();
        }
    }
}