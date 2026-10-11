using System.Collections.Generic;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Repositories;

namespace TicketEvents.Services.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _repository;

        public OrdenService(IOrdenRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Orden> ObtenerOrdenesPorUsuario(string usuarioId)
        {
            return _repository.GetByUsuarioId(usuarioId);
        }

        public Orden ObtenerOrdenConDetalles(int ordenId)
        {
            return _repository.GetByIdWithDetails(ordenId);
        }

        public void CrearOrden(Orden orden)
        {
            _repository.Add(orden);
            _repository.SaveChanges();
        }

        public void GuardarCambios()
        {
            _repository.SaveChanges();
        }

        public IEnumerable<Evento> ObtenerEventosComprados(string usuarioId)
        {
            return _repository.GetEventosCompradosPorUsuario(usuarioId);
        }

        public Orden Comprar(string usuarioId, IList<CompraItem> items)
        {
            if (items == null || items.Count == 0)
                throw new System.InvalidOperationException("El carrito está vacío.");
            return _repository.Comprar(usuarioId, items);
        }
    }
}
