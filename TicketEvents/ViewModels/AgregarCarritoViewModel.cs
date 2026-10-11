using System.ComponentModel.DataAnnotations;

namespace TicketEvents.ViewModels
{
    /// <summary>Datos que envía el cliente a POST api/carrito.</summary>
    public class AgregarCarritoViewModel
    {
        // Se usa [Range] (y no [Required]) porque Web API no admite [Required] en tipos valor.
        [Range(1, int.MaxValue, ErrorMessage = "Evento no válido.")]
        public int EventoId { get; set; }

        [Range(1, 20, ErrorMessage = "La cantidad debe estar entre 1 y 20.")]
        public int Cantidad { get; set; }
    }
}
