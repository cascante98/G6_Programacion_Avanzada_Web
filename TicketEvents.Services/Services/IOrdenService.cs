using System.Collections.Generic;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Repositories;

namespace TicketEvents.Services.Services
{
    public interface IOrdenService
    {
        IEnumerable<Orden> ObtenerOrdenesPorUsuario(string usuarioId);
        Orden ObtenerOrdenConDetalles(int ordenId);
        void CrearOrden(Orden orden);
        void GuardarCambios();
        Orden Comprar(string usuarioId, IList<CompraItem> items);
        IEnumerable<Evento> ObtenerEventosComprados(string usuarioId);
    }
}
