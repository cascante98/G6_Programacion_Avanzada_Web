using TicketEvents.Repository.Repositories;
namespace TicketEvents.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        public DashboardService(IDashboardRepository repository) { _repository = repository; }
        public object ObtenerEstadisticas() { return _repository.GetEstadisticas(); }
        public object ObtenerVentasPorEvento() { return _repository.GetVentasPorEvento(); }
        public object ObtenerVentasPorSemana() { return _repository.GetVentasPorSemana(); }
        public object ObtenerEventosBajoAforo() { return _repository.GetEventosBajoAforo(); }
    }
}
