namespace TicketEvents.Entities.Models
{
    public class DetalleOrden
    {
        public int DetalleOrdenId { get; set; }

        public int OrdenId { get; set; }
        public virtual Orden Orden { get; set; }

        public int EventoId { get; set; }
        public virtual Evento Evento { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public virtual System.Collections.Generic.ICollection<Ticket> Tickets { get; set; }
    }
}
