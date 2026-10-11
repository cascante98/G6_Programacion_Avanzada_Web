using System.Web.Mvc;
using TicketEvents.Helpers;

namespace TicketEvents.Controllers
{
    /// <summary>
    /// Muestra el carrito. Agregar, quitar y comprar se hace por AJAX contra la Web API
    /// (api/carrito y api/compras). Solo para Asociados: visitantes y administradores no compran.
    /// </summary>
    [Authorize(Roles = "Asociado")]
    public class CarritoController : Controller
    {
        public ActionResult Index()
        {
            return View(CarritoSesion.Obtener(Session));
        }
    }
}
