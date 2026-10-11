using System;
using System.ComponentModel.DataAnnotations;

namespace TicketEvents.Entities.Models
{
    public class Resena
    {
        public int ResenaId { get; set; }

        [Required]
        public string Comentario { get; set; }

        [Range(1, 5)]
        public int Calificacion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Estado { get; set; } 

        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public int EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}