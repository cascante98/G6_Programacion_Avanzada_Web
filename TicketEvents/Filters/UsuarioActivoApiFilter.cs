using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.AspNet.Identity;
using TicketEvents.Services.Services;

namespace TicketEvents.Filters
{
    /// <summary>
    /// Filtro de autorización global de la Web API (Unidad 6): rechaza con 401
    /// cualquier llamada de un usuario que el administrador haya desactivado.
    /// </summary>
    public class UsuarioActivoApiFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal;
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                return;

            var servicio = actionContext.Request.GetDependencyScope().GetService(typeof(IUsuarioService)) as IUsuarioService;
            if (servicio != null && !servicio.EstaActivo(principal.Identity.GetUserId()))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Tu cuenta está inactiva. Contacta al administrador.");
            }
        }
    }
}
