using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using TicketEvents.Services.Services;

namespace TicketEvents.Filters
{
    /// <summary>
    /// Filtro de autorización global MVC (Unidad 6): si el administrador desactiva a un
    /// usuario que tiene la sesión abierta, en su siguiente solicitud se cierra la sesión.
    /// </summary>
    public class UsuarioActivoFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            var user = filterContext.HttpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return;

            var servicio = DependencyResolver.Current.GetService<IUsuarioService>();
            if (servicio == null || servicio.EstaActivo(user.Identity.GetUserId()))
                return;

            filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            filterContext.Controller.TempData["Error"] = "Tu cuenta está inactiva. Contacta al administrador.";
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
        }
    }
}
