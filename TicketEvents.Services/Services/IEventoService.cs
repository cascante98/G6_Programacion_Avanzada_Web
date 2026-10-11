using System.Collections.Generic;
using TicketEvents.Entities.Models;

namespace TicketEvents.Services.Services
{
    public interface IEventoService
    {
        IEnumerable<Evento> ObtenerEventos();
        /// <summary>Eventos activos con fecha futura (lo que ve el público).</summary>
        IEnumerable<Evento> ObtenerCartelera();
        Evento ObtenerEventoPorId(int id);
        Evento ObtenerEventoConImagenes(int id);
        Imagen ObtenerImagen(int imagenId);
        Imagen ObtenerImagenPrincipal(int eventoId);
        void CrearEvento(Evento evento);
        /// <summary>Actualiza datos e imágenes respetando las entradas ya vendidas y el RowVersion.</summary>
        void ActualizarEvento(Evento datos, IList<byte[]> nuevasImagenes, byte[] rowVersion);
        /// <summary>Elimina el evento. Si ya tiene ventas, lo desactiva y devuelve false.</summary>
        bool EliminarEvento(int id);
        void GuardarCambios();
    }
}
