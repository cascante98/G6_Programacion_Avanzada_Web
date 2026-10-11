namespace TicketEvents.Repository.Repositories
{
    public interface IDashboardRepository
    {
        object GetEstadisticas();
        object GetVentasPorEvento();
        object GetVentasPorSemana();
        object GetEventosBajoAforo();
    }
}
