using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;

namespace TicketEvents.Repository.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly TicketEventsContext _context;

        public OrdenRepository(TicketEventsContext context)
        {
            _context = context;
        }

        public IEnumerable<Orden> GetByUsuarioId(string usuarioId)
        {
            return _context.Ordenes
                .Include(o => o.DetallesOrden)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.Fecha)
                .ToList();
        }

        public Orden GetByIdWithDetails(int id)
        {
            return _context.Ordenes
                .Include(o => o.DetallesOrden.Select(d => d.Evento))
                .Include(o => o.DetallesOrden.Select(d => d.Tickets))
                .FirstOrDefault(o => o.OrdenId == id);
        }

        public void Add(Orden orden)
        {
            _context.Ordenes.Add(orden);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool UsuarioComproEvento(string usuarioId, int eventoId)
        {
            return _context.DetallesOrden.Any(d => d.EventoId == eventoId && d.Orden.UsuarioId == usuarioId);
        }

        public IEnumerable<Evento> GetEventosCompradosPorUsuario(string usuarioId)
        {
            var idsComprados = _context.DetallesOrden
                .Where(d => d.Orden.UsuarioId == usuarioId)
                .Select(d => d.EventoId)
                .Distinct();

            return _context.Eventos
                .Where(e => idsComprados.Contains(e.EventoId))
                .OrderBy(e => e.Nombre)
                .ToList();
        }

        /// <summary>
        /// Compra transaccional con control de aforo (evita la sobreventa):
        /// - Transacción SERIALIZABLE + bloqueo UPDLOCK sobre cada evento: dos compras simultáneas
        ///   del mismo evento se ejecutan una después de la otra; la segunda ve el aforo ya descontado.
        /// - Todo el carrito se valida dentro de la misma transacción: si un evento no alcanza,
        ///   no se descuenta nada (rollback).
        /// - Si SQL Server elige una de ellas como víctima de deadlock (error 1205),
        ///   se reintenta hasta 3 veces; al reintentar vuelve a leer el aforo real.
        /// - Además, la columna RowVersion de Evento actúa como token de concurrencia optimista.
        /// </summary>
        public Orden Comprar(string usuarioId, IList<CompraItem> items)
        {
            const int maxIntentos = 3;
            for (var intento = 1; ; intento++)
            {
                try
                {
                    return EjecutarCompra(usuarioId, items);
                }
                catch (Exception ex) when (EsConflictoDeConcurrencia(ex))
                {
                    if (intento >= maxIntentos)
                        throw new InvalidOperationException("Hay mucha demanda en este momento. No se descontó ninguna entrada; intenta de nuevo.");

                    // Espera breve antes de reintentar; el reintento vuelve a leer el aforo real.
                    System.Threading.Thread.Sleep(100 * intento);
                }
            }
        }

        /// <summary>
        /// EF envuelve la SqlException dentro de otras excepciones (DbUpdateException,
        /// EntityCommandExecutionException...), por eso se recorre toda la cadena.
        /// 1205 = deadlock; DbUpdateConcurrencyException = RowVersion cambió.
        /// </summary>
        private static bool EsConflictoDeConcurrencia(Exception ex)
        {
            for (var actual = ex; actual != null; actual = actual.InnerException)
            {
                if (actual is System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                    return true;
                var sql = actual as SqlException;
                if (sql != null && sql.Number == 1205)
                    return true;
            }
            return false;
        }

        private static Orden EjecutarCompra(string usuarioId, IList<CompraItem> items)
        {
            using (var context = new TicketEventsContext())
            {
                // La compra solo modifica EntradasDisponibles; se omite la validación completa de la entidad.
                context.Configuration.ValidateOnSaveEnabled = false;

                using (var transaccion = context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var orden = new Orden { Fecha = DateTime.Now, UsuarioId = usuarioId, DetallesOrden = new List<DetalleOrden>() };

                    // Se procesan en orden de EventoId para que dos carritos con los mismos eventos
                    // tomen los bloqueos en el mismo orden (evita deadlocks).
                    foreach (var item in items.OrderBy(i => i.EventoId))
                    {
                        // UPDLOCK: bloqueo pesimista de la fila. Si otro asociado está comprando el mismo
                        // evento, esta consulta ESPERA a que su transacción termine y luego lee el aforo real.
                        var evento = context.Eventos
                            .SqlQuery("SELECT * FROM dbo.Eventoes WITH (UPDLOCK, ROWLOCK) WHERE EventoId = @p0 AND Estado = 1", item.EventoId)
                            .SingleOrDefault();

                        if (evento == null || evento.FechaHora < DateTime.Now)
                            throw new InvalidOperationException("Uno de los eventos del carrito ya no está disponible.");
                        if (item.Cantidad < 1)
                            throw new InvalidOperationException("La cantidad de entradas no es válida.");
                        if (item.Cantidad > evento.EntradasDisponibles)
                            throw new InvalidOperationException("No hay suficientes entradas para \"" + evento.Nombre + "\". Quedan "
                                + evento.EntradasDisponibles + " disponibles. No se realizó ningún cobro.");

                        evento.EntradasDisponibles -= item.Cantidad;

                        var detalle = new DetalleOrden
                        {
                            EventoId = evento.EventoId,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = evento.Precio,
                            Tickets = new List<Ticket>()
                        };
                        for (var n = 0; n < item.Cantidad; n++)
                            detalle.Tickets.Add(new Ticket { Codigo = "TE-" + Guid.NewGuid().ToString("N").Substring(0, 17).ToUpperInvariant(), Usado = false });

                        orden.DetallesOrden.Add(detalle);
                        orden.Total += item.Cantidad * evento.Precio;
                    }

                    context.Ordenes.Add(orden);
                    context.SaveChanges();   // si algo falla antes del Commit, se hace rollback automático
                    transaccion.Commit();
                    return orden;
                }
            }
        }
    }
}
