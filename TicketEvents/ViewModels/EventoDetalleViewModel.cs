using System.Collections.Generic;
using System.Linq;
using TicketEvents.Entities.Models;

namespace TicketEvents.ViewModels
{
    public class EventoDetalleViewModel
    {
        public Evento Evento { get; set; }
        public IList<Resena> Resenas { get; set; }

        public double? PromedioCalificacion
        {
            get { return Resenas != null && Resenas.Any() ? Resenas.Average(r => r.Calificacion) : (double?)null; }
        }
    }
}
