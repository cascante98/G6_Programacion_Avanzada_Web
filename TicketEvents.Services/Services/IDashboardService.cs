namespace TicketEvents.Services.Services
{
    public interface IDashboardService
    {
        object ObtenerEstadisticas();
        object ObtenerVentasPorEvento();
        object ObtenerVentasPorSemana();
        object ObtenerEventosBajoAforo();
    }
}
