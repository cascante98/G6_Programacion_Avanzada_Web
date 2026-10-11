using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers
{
    [Authorize]
    public class OrdenesController : Controller
    {
        private readonly IOrdenService _ordenService;

        public OrdenesController(IOrdenService ordenService)
        {
            _ordenService = ordenService;
        }

        public ActionResult Index()
        {
            var usuarioId = User.Identity.GetUserId();
            return View(_ordenService.ObtenerOrdenesPorUsuario(usuarioId));
        }

        public ActionResult Detalle(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuarioId = User.Identity.GetUserId();
            var orden = _ordenService.ObtenerOrdenConDetalles(id.Value);

            if (orden == null || orden.UsuarioId != usuarioId)
                return HttpNotFound();

            return View(orden);
        }
    }
}