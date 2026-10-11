using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using TicketEvents.Entities.Models;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers.Api
{
    public class CrearResenaSolicitud
    {
        public int EventoId { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
    }

    [RoutePrefix("api/resenas")]
    public class ResenasApiController : ApiController
    {
        private readonly IResenaService _service;
        public ResenasApiController(IResenaService service) { _service = service; }

        [HttpGet, Route("pendientes"), Authorize(Roles = "Administrador")]
        public IHttpActionResult GetResenasPendientes()
        {
            var items = _service.ObtenerTodas()
                .Where(r => r.Estado == ResenaService.Pendiente || r.Estado == null)
                .Select(r => new { r.ResenaId, r.Comentario, r.Calificacion, r.FechaCreacion, r.EventoId, EventoNombre = r.Evento.Nombre, r.UsuarioId, UsuarioNombre = r.Usuario.Nombre });
            return Ok(items);
        }

        [HttpPut, Route("{id:int}/aprobar"), Authorize(Roles = "Administrador")]
        public IHttpActionResult AprobarResena(int id)
        {
            if (_service.ObtenerPorId(id) == null) return NotFound();
            _service.CambiarEstado(id, ResenaService.Aprobada);
            return Ok(new { mensaje = "Reseña aprobada: ya es visible en el detalle del evento.", estado = ResenaService.Aprobada });
        }

        [HttpPut, Route("{id:int}/rechazar"), Authorize(Roles = "Administrador")]
        public IHttpActionResult RechazarResena(int id)
        {
            if (_service.ObtenerPorId(id) == null) return NotFound();
            _service.CambiarEstado(id, ResenaService.Rechazada);
            return Ok(new { mensaje = "Reseña rechazada.", estado = ResenaService.Rechazada });
        }

        [HttpPost, Route(""), Authorize(Roles = "Asociado")]
        public IHttpActionResult CrearResena(CrearResenaSolicitud solicitud)
        {
            if (solicitud == null || solicitud.EventoId <= 0 || solicitud.Calificacion < 1 || solicitud.Calificacion > 5 || string.IsNullOrWhiteSpace(solicitud.Comentario))
                return BadRequest("Selecciona un evento, una calificación entre 1 y 5 y escribe un comentario.");
            if (solicitud.Comentario.Length > 500)
                return BadRequest("El comentario no puede superar los 500 caracteres.");

            var resena = new Resena
            {
                EventoId = solicitud.EventoId,
                Calificacion = solicitud.Calificacion,
                Comentario = solicitud.Comentario.Trim(),
                UsuarioId = User.Identity.GetUserId()
            };
            // Si el asociado no compró el evento, el servicio lanza una excepción
            // que ApiExceptionFilter convierte en 400 con el mensaje.
            _service.Crear(resena);
            return Ok(new { mensaje = "Reseña enviada correctamente. Quedará visible cuando un administrador la apruebe.", resenaId = resena.ResenaId });
        }
    }
}
