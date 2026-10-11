using System;
using System.Linq;
using System.Web.Http;
using TicketEvents.Helpers;
using TicketEvents.Services.Services;
using TicketEvents.ViewModels;

namespace TicketEvents.Controllers.Api
{
    /// <summary>
    /// Carrito vía AJAX (requisito: "agregar al carrito de forma asíncrona (POST)").
    /// Solo el rol Asociado puede usarlo; el administrador no compra.
    /// </summary>
    [RoutePrefix("api/carrito"), Authorize(Roles = "Asociado")]
    public class CarritoApiController : ApiController
    {
        private readonly IEventoService _eventoService;
        public CarritoApiController(IEventoService eventoService) { _eventoService = eventoService; }

        [HttpGet, Route("")]
        public IHttpActionResult Obtener()
        {
            var sesion = CarritoSesion.Actual;
            var carrito = CarritoSesion.Obtener(sesion);
            return Ok(new
            {
                items = carrito.Select(x => new { x.EventoId, x.Nombre, x.Cantidad, x.PrecioUnitario, x.Subtotal }),
                totalEntradas = CarritoSesion.TotalEntradas(sesion),
                total = carrito.Sum(x => x.Subtotal)
            });
        }

        [HttpPost, Route("")]
        public IHttpActionResult Agregar(AgregarCarritoViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest("Seleccione una cantidad entre 1 y " + CarritoSesion.MaximoPorEvento + ".");

            var evento = _eventoService.ObtenerEventoPorId(model.EventoId);
            if (evento == null || !evento.Estado || evento.FechaHora < DateTime.Now)
                return BadRequest("El evento ya no se encuentra disponible.");

            var sesion = CarritoSesion.Actual;
            var carrito = CarritoSesion.Obtener(sesion);
            var item = carrito.FirstOrDefault(x => x.EventoId == evento.EventoId);
            var cantidadEnCarrito = item == null ? 0 : item.Cantidad;

            if (cantidadEnCarrito + model.Cantidad > CarritoSesion.MaximoPorEvento)
                return BadRequest("Máximo " + CarritoSesion.MaximoPorEvento + " entradas por evento. Ya tienes " + cantidadEnCarrito + " en el carrito.");

            // Nota: aquí NO se valida ni se descuenta el aforo (requisito 3.3).
            // Eso ocurre únicamente al confirmar la compra, dentro de una transacción.
            if (item == null)
            {
                carrito.Add(new CarritoItemViewModel
                {
                    EventoId = evento.EventoId,
                    Nombre = evento.Nombre,
                    FechaHora = evento.FechaHora,
                    Cantidad = model.Cantidad,
                    PrecioUnitario = evento.Precio
                });
            }
            else
            {
                item.Cantidad += model.Cantidad;
            }

            return Ok(new
            {
                mensaje = model.Cantidad + " entrada(s) de \"" + evento.Nombre + "\" agregadas al carrito.",
                totalEntradas = CarritoSesion.TotalEntradas(sesion)
            });
        }

        [HttpDelete, Route("{eventoId:int}")]
        public IHttpActionResult Eliminar(int eventoId)
        {
            var sesion = CarritoSesion.Actual;
            var carrito = CarritoSesion.Obtener(sesion);
            carrito.RemoveAll(x => x.EventoId == eventoId);
            return Ok(new
            {
                mensaje = "Evento eliminado del carrito.",
                totalEntradas = CarritoSesion.TotalEntradas(sesion),
                total = carrito.Sum(x => x.Subtotal)
            });
        }
    }
}
