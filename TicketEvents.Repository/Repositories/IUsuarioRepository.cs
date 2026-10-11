using TicketEvents.Entities.Models;

namespace TicketEvents.Repository.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario GetById(string id);
        void Update(Usuario usuario);
        void SaveChanges();
        System.Collections.Generic.IList<Usuario> GetAll();
        bool TieneOrdenes(string usuarioId);
    }
}