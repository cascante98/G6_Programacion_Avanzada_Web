using System.Web.Http;
using TicketEvents.Filters;

namespace TicketEvents
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Filtros globales de la Web API (Unidad 6)
            config.Filters.Add(new ApiExceptionFilter());
            config.Filters.Add(new UsuarioActivoApiFilter());

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
