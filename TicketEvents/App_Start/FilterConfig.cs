using System.Web.Mvc;
using TicketEvents.Filters;

namespace TicketEvents
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RegistroExcepcionesFilter());   // registra el error
            filters.Add(new HandleErrorAttribute());        // muestra Views/Shared/Error.cshtml
            filters.Add(new UsuarioActivoFilter());         // cierra sesión de usuarios inactivos
        }
    }
}
