using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using TicketEvents.Repository.Context;
using TicketEvents.Repository.Migrations;

namespace TicketEvents
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<TicketEventsContext, Configuration>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// La Web API no usa sesión por defecto. Se habilita para las rutas /api
        /// porque el carrito vive en la sesión del usuario.
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            var ruta = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
            if (ruta != null && ruta.StartsWith("~/api", System.StringComparison.OrdinalIgnoreCase))
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
    }
}
