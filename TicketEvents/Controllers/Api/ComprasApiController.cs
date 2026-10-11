using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using TicketEvents.Helpers;
using TicketEvents.Repository.Repositories;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers.Api
{
    [RoutePrefix("api/compras"), Authorize(Roles = "Asociado")]
    public class ComprasApiController : ApiController
    {
        private readonly IOrdenService _service;
        public ComprasApiController(IOrdenService service) { _service = service; }

        /// <summary>
        /// Confirma la compra del carrito guardado en la sesión (el cliente no envía
        /// precios ni cantidades, así no puede manipularlos). Si el aforo no alcanza,
        /// el servicio lanza InvalidOperationException y ApiExceptionFilter responde 400.
        /// </summary>
        [HttpPost, Route("")]
        public IHttpActionResult Comprar()
        {
            var sesion = CarritoSesion.Actual;
            var carrito = CarritoSesion.Obtener(sesion);
            if (!carrito.Any())
                return BadRequest("El carrito está vacío.");

            var items = carrito.Select(x => new CompraItem { EventoId = x.EventoId, Cantidad = x.Cantidad }).ToList();
            var orden = _service.Comprar(User.Identity.GetUserId(), items);

            CarritoSesion.Limpiar(sesion);
            return Ok(new { mensaje = "Compra completada correctamente.", ordenId = orden.OrdenId });
        }
    }
}
