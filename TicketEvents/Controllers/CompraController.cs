using System.Web.Mvc;
using TicketEvents.Helpers;

namespace TicketEvents.Controllers
{
    /// <summary>
    /// La compra se confirma por AJAX en POST api/compras. Esta acción solo redirige
    /// al detalle de la orden recién creada.
    /// </summary>
    [Authorize(Roles = "Asociado")]
    public class CompraController : Controller
    {
        [HttpGet]
        public ActionResult FinalizadaApi(int id)
        {
            CarritoSesion.Limpiar(Session);
            TempData["Mensaje"] = "¡Compra completada! Estos son tus tickets.";
            return RedirectToAction("Detalle", "Ordenes", new { id });
        }
    }
}
