using System.Web.Mvc;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers
{
    /// <summary>Lista las reseñas. Aprobar/rechazar se hace por AJAX (PUT api/resenas/{id}/...).</summary>
    [Authorize(Roles = "Administrador")]
    public class ResenasAdminController : Controller
    {
        private readonly IResenaService _service;
        public ResenasAdminController(IResenaService service) { _service = service; }
        public ActionResult Index() { return View(_service.ObtenerTodas()); }
    }
}
