using System.ComponentModel.DataAnnotations;

namespace TicketEvents.Entities.Models
{
    public class Imagen
    {
        public int ImagenId { get; set; }

        [Required]
        public byte[] DatosImagen { get; set; } 

        public int EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}