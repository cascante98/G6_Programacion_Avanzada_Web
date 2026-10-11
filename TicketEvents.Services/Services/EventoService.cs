using System;
using System.Collections.Generic;
using System.Linq;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Repositories;

namespace TicketEvents.Services.Services
{
    public class EventoService : IEventoService
    {
        public const int MinimoImagenes = 3;
        private readonly IEventoRepository _repository;

        public EventoService(IEventoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Evento> ObtenerEventos()
        {
            return _repository.GetAll();
        }

        public IEnumerable<Evento> ObtenerCartelera()
        {
            var ahora = DateTime.Now;
            return _repository.GetAll()
                .Where(e => e.Estado && e.FechaHora >= ahora)
                .OrderBy(e => e.FechaHora)
                .ToList();
        }

        public Evento ObtenerEventoPorId(int id)
        {
            return _repository.GetById(id);
        }

        public Evento ObtenerEventoConImagenes(int id)
        {
            return _repository.GetWithImages(id);
        }

        public Imagen ObtenerImagen(int imagenId)
        {
            return _repository.GetImagenById(imagenId);
        }

        public Imagen ObtenerImagenPrincipal(int eventoId)
        {
            return _repository.GetPrimeraImagen(eventoId);
        }

        public void CrearEvento(Evento evento)
        {
            if (evento.Imagenes == null || evento.Imagenes.Count < MinimoImagenes)
                throw new InvalidOperationException("Debe subir un mínimo de " + MinimoImagenes + " imágenes para el evento.");

            evento.EntradasDisponibles = evento.AforoTotal;
            _repository.Add(evento);
            _repository.SaveChanges();
        }

        public void ActualizarEvento(Evento datos, IList<byte[]> nuevasImagenes, byte[] rowVersion)
        {
            var eventoDB = _repository.GetWithImages(datos.EventoId);
            if (eventoDB == null)
                throw new InvalidOperationException("El evento no existe.");

            // Regla de negocio: las entradas vendidas se conservan; si cambia el aforo,
            // las disponibles se recalculan. No se permite un aforo menor a lo ya vendido.
            var vendidas = eventoDB.AforoTotal - eventoDB.EntradasDisponibles;
            if (datos.AforoTotal < vendidas)
                throw new InvalidOperationException("El aforo no puede ser menor a las " + vendidas + " entradas ya vendidas.");

            eventoDB.Nombre = datos.Nombre;
            eventoDB.Descripcion = datos.Descripcion;
            eventoDB.Categoria = datos.Categoria;
            eventoDB.FechaHora = datos.FechaHora;
            eventoDB.Ubicacion = datos.Ubicacion;
            eventoDB.Precio = datos.Precio;
            eventoDB.AforoTotal = datos.AforoTotal;
            eventoDB.EntradasDisponibles = datos.AforoTotal - vendidas;
            eventoDB.Estado = datos.Estado;

            if (nuevasImagenes != null)
            {
                foreach (var bytes in nuevasImagenes)
                    eventoDB.Imagenes.Add(new Imagen { DatosImagen = bytes });
            }

            // Lanza DbUpdateConcurrencyException si otro usuario (o una compra) modificó el evento.
            _repository.UpdateWithRowVersion(eventoDB, rowVersion);
        }

        public bool EliminarEvento(int id)
        {
            if (_repository.TieneVentas(id))
            {
                // No se borra para conservar el historial de órdenes y tickets.
                var evento = _repository.GetById(id);
                if (evento != null)
                {
                    evento.Estado = false;
                    _repository.SaveChanges();
                }
                return false;
            }

            _repository.Delete(id);
            _repository.SaveChanges();
            return true;
        }

        public void GuardarCambios()
        {
            _repository.SaveChanges();
        }
    }
}
