using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;

namespace TicketEvents.Repository.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly TicketEventsContext _context;

        public UsuarioRepository(TicketEventsContext context)
        {
            _context = context;
        }

        public Usuario GetById(string id)
        {
            return _context.Users.Find(id);
        }

        public void Update(Usuario usuario)
        {
            _context.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
        }

        public System.Collections.Generic.IList<Usuario> GetAll()
        {
            return System.Linq.Enumerable.ToList(System.Linq.Queryable.OrderBy(_context.Users, u => u.Nombre));
        }

        public bool TieneOrdenes(string usuarioId)
        {
            return System.Linq.Queryable.Any(_context.Ordenes, o => o.UsuarioId == usuarioId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}