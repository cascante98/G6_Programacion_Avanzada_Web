using System.Diagnostics;
using System.Web.Mvc;

namespace TicketEvents.Filters
{
    /// <summary>
    /// Filtro global de excepciones MVC (Unidad 6): registra toda excepción no controlada
    /// (ventana Salida de Visual Studio). Luego HandleErrorAttribute muestra la vista Error.
    /// </summary>
    public class RegistroExcepcionesFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var ruta = filterContext.RouteData.Values;
            Trace.TraceError("[MVC] {0}/{1}: {2}", ruta["controller"], ruta["action"], filterContext.Exception);
        }
    }
}
