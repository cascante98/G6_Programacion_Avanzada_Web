using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketEvents.Services.Services;
using TicketEvents.ViewModels;

namespace TicketEvents.Controllers
{
    /// <summary>Cartelera pública: cualquier visitante puede navegarla.</summary>
    public class EventoController : Controller
    {
        private readonly IEventoService _eventoService;
        private readonly IResenaService _resenaService;

        public EventoController(IEventoService eventoService, IResenaService resenaService)
        {
            _eventoService = eventoService;
            _resenaService = resenaService;
        }

        public ActionResult Index()
        {
            return View(_eventoService.ObtenerCartelera());
        }

        /// <summary>Detalle del evento: galería de imágenes y reseñas aprobadas.</summary>
        public ActionResult Detalle(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            var evento = _eventoService.ObtenerEventoConImagenes(id.Value);
            if (evento == null || (!evento.Estado && !User.IsInRole("Administrador")))
                return HttpNotFound();

            return View(new EventoDetalleViewModel
            {
                Evento = evento,
                Resenas = _resenaService.ObtenerAprobadasPorEvento(evento.EventoId)
            });
        }

        /// <summary>Primera imagen del evento (portada de la tarjeta).</summary>
        public ActionResult ImagenPrincipal(int id)
        {
            var imagen = _eventoService.ObtenerImagenPrincipal(id);
            if (imagen == null || imagen.DatosImagen == null)
                return HttpNotFound();

            CachearRespuesta();
            return File(imagen.DatosImagen, DetectarTipo(imagen.DatosImagen));
        }

        /// <summary>Devuelve una imagen específica por su ImagenId (almacenada en la BD).</summary>
        public ActionResult Imagen(int id)
        {
            var imagen = _eventoService.ObtenerImagen(id);
            if (imagen == null || imagen.DatosImagen == null)
                return HttpNotFound();

            CachearRespuesta();
            return File(imagen.DatosImagen, DetectarTipo(imagen.DatosImagen));
        }

        private void CachearRespuesta()
        {
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(TimeSpan.FromMinutes(30));
        }

        private static string DetectarTipo(byte[] datos)
        {
            if (datos.Length >= 4 && datos[0] == 0x89 && datos[1] == 0x50 && datos[2] == 0x4E && datos[3] == 0x47)
                return "image/png";
            if (datos.Length >= 3 && datos[0] == 0xFF && datos[1] == 0xD8 && datos[2] == 0xFF)
                return "image/jpeg";
            if (datos.Length >= 6 && datos[0] == 0x47 && datos[1] == 0x49 && datos[2] == 0x46)
                return "image/gif";
            return "application/octet-stream";
        }
    }
}
