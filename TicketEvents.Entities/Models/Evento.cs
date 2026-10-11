using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketEvents.Entities.Models
{
    public class Evento : IValidatableObject
    {
        public int EventoId { get; set; }

        /// <summary>
        /// Código legible del evento (ej. EVT-0007). Se deriva de la llave primaria,
        /// por lo que es único y no requiere una columna adicional en la base de datos.
        /// </summary>
        [NotMapped]
        [Display(Name = "Código")]
        public string Codigo
        {
            get { return EventoId > 0 ? "EVT-" + EventoId.ToString("D4") : "(nuevo)"; }
        }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; }

        [Display(Name = "Lugar / recinto")]
        public string Ubicacion { get; set; }

        [Display(Name = "Precio")]
        [Range(typeof(decimal), "1", "10000000", ErrorMessage = "El precio debe estar entre 1 y 10 000 000.")]
        public decimal Precio { get; set; }

        [Display(Name = "Aforo Total")]
        [Range(1, 100000, ErrorMessage = "El aforo debe estar entre 1 y 100 000.")]
        public int AforoTotal { get; set; }

        [Display(Name = "Entradas Disponibles")]
        [Range(0, 100000, ErrorMessage = "Las entradas disponibles no pueden ser negativas.")]
        public int EntradasDisponibles { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Imagen> Imagenes { get; set; }
        public virtual ICollection<Resena> Resenas { get; set; }
        public virtual ICollection<DetalleOrden> DetallesOrden { get; set; }

        /// <summary>
        /// Validaciones de negocio adicionales. Se implementan con IValidatableObject
        /// (y no con [Required]) para no alterar el esquema ya migrado de la base de datos.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Categoria))
                yield return new ValidationResult("La categoría es obligatoria.", new[] { "Categoria" });

            if (string.IsNullOrWhiteSpace(Ubicacion))
                yield return new ValidationResult("El lugar o recinto es obligatorio.", new[] { "Ubicacion" });

            if (string.IsNullOrWhiteSpace(Descripcion))
                yield return new ValidationResult("La descripción es obligatoria.", new[] { "Descripcion" });

            if (FechaHora == DateTime.MinValue)
                yield return new ValidationResult("La fecha y hora del evento son obligatorias.", new[] { "FechaHora" });

            if (EntradasDisponibles > AforoTotal)
                yield return new ValidationResult("Las entradas disponibles no pueden superar el aforo total.", new[] { "EntradasDisponibles" });
        }
    }
}
