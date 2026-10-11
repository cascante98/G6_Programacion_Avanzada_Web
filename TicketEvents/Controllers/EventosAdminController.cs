using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketEvents.Entities.Models;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EventosAdminController : Controller
    {
        private const int TamanoMaximoImagen = 2 * 1024 * 1024; // 2 MB
        private static readonly string[] TiposPermitidos = { "image/jpeg", "image/png", "image/gif" };

        private readonly IEventoService _eventoService;

        public EventosAdminController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        public ActionResult Index()
        {
            return View(_eventoService.ObtenerEventos().OrderBy(e => e.FechaHora).ToList());
        }

        public ActionResult Dashboard() { return View(); }

        public ActionResult Create()
        {
            return View(new Evento { FechaHora = DateTime.Today.AddDays(7).AddHours(19) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Evento evento, HttpPostedFileBase[] imagenesSubidas)
        {
            var imagenes = LeerImagenes(imagenesSubidas);
            if (imagenes.Count < EventoService.MinimoImagenes)
                ModelState.AddModelError("", "Debe subir un mínimo de " + EventoService.MinimoImagenes + " imágenes válidas (JPG, PNG o GIF de hasta 2 MB).");

            if (!ModelState.IsValid)
                return View(evento);

            try
            {
                evento.Estado = true;
                evento.Imagenes = imagenes.Select(b => new Imagen { DatosImagen = b }).ToList();
                _eventoService.CrearEvento(evento);
                TempData["Mensaje"] = "Evento \"" + evento.Nombre + "\" creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(evento);
            }
        }

        public ActionResult Edit(int id)
        {
            var evento = _eventoService.ObtenerEventoConImagenes(id);
            if (evento == null) return HttpNotFound();
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Evento evento, HttpPostedFileBase[] imagenesSubidas)
        {
            // Las entradas disponibles las calcula el servicio a partir de lo ya vendido.
            ModelState.Remove("EntradasDisponibles");

            if (ModelState.IsValid)
            {
                try
                {
                    _eventoService.ActualizarEvento(evento, LeerImagenes(imagenesSubidas), evento.RowVersion);
                    TempData["Mensaje"] = "Evento actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "El evento fue modificado por otro usuario (o se vendieron entradas) mientras editaba. Recargue la página y vuelva a intentarlo.");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Recarga imágenes y entradas actuales para volver a mostrar el formulario.
            var actual = _eventoService.ObtenerEventoConImagenes(evento.EventoId);
            if (actual != null)
            {
                evento.Imagenes = actual.Imagenes;
                evento.EntradasDisponibles = actual.EntradasDisponibles;
            }
            return View(evento);
        }

        public ActionResult Delete(int id)
        {
            var evento = _eventoService.ObtenerEventoPorId(id);
            if (evento == null) return HttpNotFound();
            return View(evento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var eliminado = _eventoService.EliminarEvento(id);
            TempData["Mensaje"] = eliminado
                ? "Evento eliminado correctamente."
                : "El evento tiene entradas vendidas, así que no se borró: se marcó como Inactivo para conservar el historial.";
            return RedirectToAction("Index");
        }

        private static List<byte[]> LeerImagenes(IEnumerable<HttpPostedFileBase> archivos)
        {
            var resultado = new List<byte[]>();
            if (archivos == null) return resultado;

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.ContentLength == 0 || archivo.ContentLength > TamanoMaximoImagen)
                    continue;
                if (!TiposPermitidos.Contains((archivo.ContentType ?? "").ToLowerInvariant()))
                    continue;

                using (var lector = new BinaryReader(archivo.InputStream))
                {
                    resultado.Add(lector.ReadBytes(archivo.ContentLength));
                }
            }
            return resultado;
        }
    }
}
