using System;
using System.Collections.Generic;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Repositories;

namespace TicketEvents.Services.Services
{
    public class ResenaService : IResenaService
    {
        public const string Pendiente = "Pendiente";
        public const string Aprobada = "Aprobada";
        public const string Rechazada = "Rechazada";

        private readonly IResenaRepository _repository;
        private readonly IOrdenRepository _ordenRepository;

        public ResenaService(IResenaRepository repository, IOrdenRepository ordenRepository)
        {
            _repository = repository;
            _ordenRepository = ordenRepository;
        }

        public IList<Resena> ObtenerTodas() { return _repository.GetAll(); }

        public IList<Resena> ObtenerAprobadasPorEvento(int eventoId) { return _repository.GetAprobadasPorEvento(eventoId); }

        public Resena ObtenerPorId(int id) { return _repository.GetById(id); }

        public void CambiarEstado(int id, string estado)
        {
            if (estado != Aprobada && estado != Rechazada)
                throw new InvalidOperationException("Estado de reseña no válido.");

            var item = _repository.GetById(id);
            if (item == null) return;
            item.Estado = estado;
            _repository.Update(item);
            _repository.SaveChanges();
        }

        public void Crear(Resena item)
        {
            // Solo pueden reseñar quienes compraron (asistieron) al evento.
            if (!_ordenRepository.UsuarioComproEvento(item.UsuarioId, item.EventoId))
                throw new InvalidOperationException("Solo puedes reseñar eventos para los que compraste entradas.");

            if (_repository.ExisteResena(item.UsuarioId, item.EventoId))
                throw new InvalidOperationException("Ya enviaste una reseña para este evento.");

            item.Estado = Pendiente;
            item.FechaCreacion = DateTime.Now;
            _repository.Add(item);
            _repository.SaveChanges();
        }
    }
}
