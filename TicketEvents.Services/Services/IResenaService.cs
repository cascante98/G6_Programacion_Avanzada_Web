using System.Collections.Generic;
using TicketEvents.Entities.Models;

namespace TicketEvents.Services.Services
{
    public interface IResenaService
    {
        IList<Resena> ObtenerTodas();
        IList<Resena> ObtenerAprobadasPorEvento(int eventoId);
        Resena ObtenerPorId(int id);
        void CambiarEstado(int id, string estado);
        /// <summary>Crea la reseña solo si el asociado compró entradas del evento.</summary>
        void Crear(Resena resena);
    }
}
