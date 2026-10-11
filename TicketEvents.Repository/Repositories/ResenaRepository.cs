using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;
namespace TicketEvents.Repository.Repositories
{
    public class ResenaRepository : IResenaRepository
    {
        private readonly TicketEventsContext _context;
        public ResenaRepository(TicketEventsContext context) { _context = context; }
        public IList<Resena> GetAll() { return _context.Resenas.Include(r => r.Usuario).Include(r => r.Evento).OrderByDescending(r => r.FechaCreacion).ToList(); }
        public Resena GetById(int id) { return _context.Resenas.Find(id); }
        public void Update(Resena item) { _context.Entry(item).State = EntityState.Modified; }
        public void SaveChanges() { _context.SaveChanges(); }
        public void Add(Resena item) { _context.Resenas.Add(item); }
        public IList<Resena> GetAprobadasPorEvento(int eventoId)
        {
            return _context.Resenas.Include(r => r.Usuario)
                .Where(r => r.EventoId == eventoId && r.Estado == "Aprobada")
                .OrderByDescending(r => r.FechaCreacion)
                .ToList();
        }
        public bool ExisteResena(string usuarioId, int eventoId)
        {
            return _context.Resenas.Any(r => r.UsuarioId == usuarioId && r.EventoId == eventoId && r.Estado != "Rechazada");
        }
    }
}
