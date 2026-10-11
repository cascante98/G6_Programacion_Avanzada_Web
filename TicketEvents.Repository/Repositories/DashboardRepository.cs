using System;
using System.Data.Entity;
using System.Linq;
using TicketEvents.Repository.Context;

namespace TicketEvents.Repository.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        /// <summary>Un evento está "próximo a agotarse" cuando le queda 10 % o menos de su aforo.</summary>
        private const int PorcentajeBajoAforo = 10;

        private readonly TicketEventsContext _context;
        public DashboardRepository(TicketEventsContext context) { _context = context; }

        public object GetEstadisticas()
        {
            var ahora = DateTime.Now;
            var eventos = _context.Eventos.ToList();
            var usuariosActivos = _context.Users.Count(u => u.Estado);
            var usuariosInactivos = _context.Users.Count(u => !u.Estado);

            return new
            {
                TotalUsuarios = usuariosActivos + usuariosInactivos,
                UsuariosActivos = usuariosActivos,
                UsuariosInactivos = usuariosInactivos,
                TotalEventos = eventos.Count,
                EventosEnCartelera = eventos.Count(e => e.Estado && e.FechaHora >= ahora),
                EventosActivos = eventos.Count(e => e.Estado),
                EventosProximos = eventos.Count(e => e.Estado && e.FechaHora >= ahora && e.FechaHora <= ahora.AddDays(30)),
                EventosBajoAforo = eventos.Count(e => EsBajoAforo(e.EntradasDisponibles, e.AforoTotal) && e.Estado && e.FechaHora >= ahora),
                TotalOrdenes = _context.Ordenes.Count(),
                EntradasVendidas = _context.DetallesOrden.Sum(d => (int?)d.Cantidad) ?? 0,
                IngresosTotales = _context.Ordenes.Sum(o => (decimal?)o.Total) ?? 0m
            };
        }

        public object GetVentasPorEvento()
        {
            return (from detalle in _context.DetallesOrden
                    join evento in _context.Eventos on detalle.EventoId equals evento.EventoId
                    group detalle by new { evento.EventoId, evento.Nombre } into ventas
                    orderby ventas.Sum(x => x.Cantidad) descending
                    select new
                    {
                        ventas.Key.EventoId,
                        EventoNombre = ventas.Key.Nombre,
                        EntradasVendidas = ventas.Sum(x => x.Cantidad),
                        TotalIngresos = ventas.Sum(x => x.Cantidad * x.PrecioUnitario)
                    }).Take(10).ToList();
        }

        /// <summary>Entradas vendidas e ingresos de los últimos 7 días (incluye días sin ventas en cero).</summary>
        public object GetVentasPorSemana()
        {
            var inicio = DateTime.Today.AddDays(-6);
            var ordenes = _context.Ordenes
                .Include(o => o.DetallesOrden)
                .Where(o => o.Fecha >= inicio)
                .ToList();

            return Enumerable.Range(0, 7)
                .Select(i => inicio.AddDays(i))
                .Select(dia => new
                {
                    Fecha = dia.ToString("dd/MM"),
                    Total = ordenes.Where(o => o.Fecha.Date == dia).Sum(o => o.Total),
                    Entradas = ordenes.Where(o => o.Fecha.Date == dia).SelectMany(o => o.DetallesOrden).Sum(d => d.Cantidad)
                })
                .ToList();
        }

        public object GetEventosBajoAforo()
        {
            var ahora = DateTime.Now;
            return _context.Eventos
                .Where(e => e.Estado && e.FechaHora >= ahora)
                .ToList()
                .Where(e => EsBajoAforo(e.EntradasDisponibles, e.AforoTotal))
                .OrderBy(e => e.EntradasDisponibles)
                .Select(e => new
                {
                    e.EventoId,
                    e.Codigo,
                    e.Nombre,
                    FechaHora = e.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                    e.EntradasDisponibles,
                    e.AforoTotal
                })
                .ToList();
        }

        private static bool EsBajoAforo(int disponibles, int aforo)
        {
            return aforo > 0 && disponibles > 0 && disponibles * 100 <= aforo * PorcentajeBajoAforo;
        }
    }
}
