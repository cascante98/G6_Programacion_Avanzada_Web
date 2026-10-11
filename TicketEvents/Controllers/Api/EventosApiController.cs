using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TicketEvents.Entities.Models;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers.Api
{
    [RoutePrefix("api/eventos")]
    public class EventosApiController : ApiController
    {
        private readonly IEventoService _eventoService;

        public EventosApiController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Administrador,Asociado")]
        public IHttpActionResult GetEventos()
        {
            var eventos = _eventoService.ObtenerEventos()
                .Where(e => e.Estado)
                .Select(e => new
                {
                    e.EventoId,
                    e.Nombre,
                    e.Categoria,
                    e.FechaHora,
                    e.Ubicacion,
                    e.Precio,
                    e.EntradasDisponibles,
                    e.AforoTotal,
                })
                .ToList();

            return Ok(eventos);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Administrador,Asociado")]
        public IHttpActionResult GetEvento(int id)
        {
            var evento = _eventoService.ObtenerEventoConImagenes(id);
            if (evento == null)
                return NotFound();

            return Ok(new
            {
                evento.EventoId,
                evento.Nombre,
                evento.Descripcion,
                evento.Categoria,
                evento.FechaHora,
                evento.Ubicacion,
                evento.Precio,
                evento.EntradasDisponibles,
                evento.AforoTotal,
                evento.Estado,
                Imagenes = evento.Imagenes != null ? evento.Imagenes.Count() : 0
            });
        }
    }
}