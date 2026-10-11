using System.ComponentModel.DataAnnotations;

namespace TicketEvents.Entities.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; }

        public int DetalleOrdenId { get; set; }
        public virtual DetalleOrden DetalleOrden { get; set; }

        public bool Usado { get; set; }
    }
}
