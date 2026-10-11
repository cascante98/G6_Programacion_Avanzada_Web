using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers
{
    [Authorize(Roles = "Asociado")]
    public class ResenasController : Controller
    {
        private readonly IOrdenService _ordenService;
        public ResenasController(IOrdenService ordenService) { _ordenService = ordenService; }

        /// <summary>Solo se listan los eventos para los que el asociado compró entradas.</summary>
        public ActionResult Crear(int? eventoId)
        {
            ViewBag.EventoSeleccionado = eventoId;
            return View(_ordenService.ObtenerEventosComprados(User.Identity.GetUserId()).ToList());
        }
    }
}
