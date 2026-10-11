using System.Collections.Generic;
using TicketEvents.Entities.Models;

namespace TicketEvents.Repository.Repositories
{
    public interface IEventoRepository
    {
        IEnumerable<Evento> GetAll();
        Evento GetById(int id);
        Evento GetWithImages(int id);
        void Add(Evento evento);
        void Delete(int id);
        void SaveChanges();
        void UpdateWithRowVersion(Evento evento, byte[] rowVersion);
        Imagen GetImagenById(int imagenId);
        Imagen GetPrimeraImagen(int eventoId);
        bool TieneVentas(int eventoId);
    }
}