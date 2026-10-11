using System.Web.Http;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers.Api
{
    [RoutePrefix("api/dashboard"), Authorize(Roles = "Administrador")]
    public class DashboardApiController : ApiController
    {
        private readonly IDashboardService _service;
        public DashboardApiController(IDashboardService service) { _service = service; }

        [HttpGet, Route("estadisticas")]
        public IHttpActionResult GetEstadisticas() { return Ok(_service.ObtenerEstadisticas()); }

        [HttpGet, Route("ventas-por-evento")]
        public IHttpActionResult GetVentasPorEvento() { return Ok(_service.ObtenerVentasPorEvento()); }

        [HttpGet, Route("ventas-por-semana")]
        public IHttpActionResult GetVentasPorSemana() { return Ok(_service.ObtenerVentasPorSemana()); }

        [HttpGet, Route("bajo-aforo")]
        public IHttpActionResult GetEventosBajoAforo() { return Ok(_service.ObtenerEventosBajoAforo()); }
    }
}
