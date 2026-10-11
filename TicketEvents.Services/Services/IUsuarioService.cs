using TicketEvents.Entities.Models;

namespace TicketEvents.Services.Services
{
    public interface IUsuarioService
    {
        Usuario ObtenerUsuarioPorId(string id);
        void ActualizarUsuario(Usuario usuario);
        System.Collections.Generic.IList<Usuario> ObtenerTodos();
        bool TieneOrdenes(string usuarioId);
        /// <summary>True si el usuario existe y está activo.</summary>
        bool EstaActivo(string usuarioId);
    }
}