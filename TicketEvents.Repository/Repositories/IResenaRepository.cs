using System.Collections.Generic;
using TicketEvents.Entities.Models;
namespace TicketEvents.Repository.Repositories
{
    public interface IResenaRepository
    {
        IList<Resena> GetAll();
        Resena GetById(int id);
        void Update(Resena resena);
        void SaveChanges();
        void Add(Resena resena);
        IList<Resena> GetAprobadasPorEvento(int eventoId);
        bool ExisteResena(string usuarioId, int eventoId);
    }
}
