using System;

namespace TicketEvents.ViewModels
{
    [Serializable]
    public class CarritoItemViewModel
    {
        public int EventoId { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaHora { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get { return Cantidad * PrecioUnitario; } }
    }
}
