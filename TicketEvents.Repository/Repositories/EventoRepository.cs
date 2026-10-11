using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;

namespace TicketEvents.Repository.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly TicketEventsContext _context;

        public EventoRepository(TicketEventsContext context)
        {
            _context = context;
        }

        public IEnumerable<Evento> GetAll()
        {
            // No se incluyen las imágenes (pueden pesar varios MB); se sirven por separado.
            return _context.Eventos.ToList();
        }

        public Imagen GetPrimeraImagen(int eventoId)
        {
            return _context.Imagenes
                .Where(i => i.EventoId == eventoId)
                .OrderBy(i => i.ImagenId)
                .FirstOrDefault();
        }

        public Evento GetById(int id)
        {
            return _context.Eventos
                .Include(e => e.Imagenes)
                .FirstOrDefault(e => e.EventoId == id);
        }

        public Evento GetWithImages(int id)
        {
            return _context.Eventos
                .Include(e => e.Imagenes)
                .FirstOrDefault(e => e.EventoId == id);
        }

        public void Add(Evento evento)
        {
            _context.Eventos.Add(evento);
        }

        public void Delete(int id)
        {
            var evento = GetById(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Imagen GetImagenById(int imagenId)
        {
            return _context.Imagenes.Find(imagenId);
        }

        public bool TieneVentas(int eventoId)
        {
            return _context.DetallesOrden.Any(d => d.EventoId == eventoId);
        }

        public void UpdateWithRowVersion(Evento evento, byte[] rowVersion)
        {
            var existing = _context.Eventos.Find(evento.EventoId);
            if (existing == null)
                throw new System.Exception("Evento no encontrado");

            _context.Entry(existing).CurrentValues.SetValues(evento);
            _context.Entry(existing).Property("RowVersion").OriginalValue = rowVersion;
            _context.SaveChanges();
        }
    }
}