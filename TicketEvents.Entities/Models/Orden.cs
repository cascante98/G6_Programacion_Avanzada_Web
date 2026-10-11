using System;
using System.Collections.Generic;

namespace TicketEvents.Entities.Models
{
    public class Orden
    {
        public int OrdenId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public string UsuarioId { get; set; } 
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<DetalleOrden> DetallesOrden { get; set; }
    }
}