using System.Collections.Generic;
using TicketEvents.Entities.Models;

namespace TicketEvents.Repository.Repositories
{
    public interface IOrdenRepository
    {
        IEnumerable<Orden> GetByUsuarioId(string usuarioId);
        Orden GetByIdWithDetails(int id);
        void Add(Orden orden);
        void SaveChanges();
        Orden Comprar(string usuarioId, IList<CompraItem> items);
        bool UsuarioComproEvento(string usuarioId, int eventoId);
        IEnumerable<Evento> GetEventosCompradosPorUsuario(string usuarioId);
    }

    public class CompraItem
    {
        public int EventoId { get; set; }
        public int Cantidad { get; set; }
    }
}
