using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace TicketEvents.Entities.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Display(Name = "Última conexión")]
        public DateTime? UltimaConexion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } 
    }
}