using TicketEvents.Entities.Models;
using TicketEvents.Repository.Repositories;

namespace TicketEvents.Services.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public Usuario ObtenerUsuarioPorId(string id)
        {
            return _repository.GetById(id);
        }

        public System.Collections.Generic.IList<Usuario> ObtenerTodos()
        {
            return _repository.GetAll();
        }

        public bool TieneOrdenes(string usuarioId)
        {
            return _repository.TieneOrdenes(usuarioId);
        }

        public bool EstaActivo(string usuarioId)
        {
            var usuario = _repository.GetById(usuarioId);
            return usuario != null && usuario.Estado;
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            _repository.Update(usuario);
            _repository.SaveChanges();
        }
    }
}