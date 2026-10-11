using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketEvents.ViewModels;

namespace TicketEvents.Helpers
{
    /// <summary>
    /// Encapsula el carrito guardado en la sesión. Lo usan tanto el controlador MVC
    /// (para mostrar el carrito) como la Web API (para agregar y comprar vía AJAX).
    /// El carrito NO descuenta aforo: el aforo se valida y descuenta solo al confirmar la compra.
    /// </summary>
    public static class CarritoSesion
    {
        public const string Clave = "Carrito";
        public const int MaximoPorEvento = 20;

        /// <summary>Sesión de la solicitud actual (usada desde la Web API).</summary>
        public static HttpSessionStateBase Actual
        {
            get
            {
                var sesion = HttpContext.Current == null ? null : HttpContext.Current.Session;
                if (sesion == null)
                    throw new InvalidOperationException("La sesión no está disponible para esta solicitud.");
                return new HttpSessionStateWrapper(sesion);
            }
        }

        public static List<CarritoItemViewModel> Obtener(HttpSessionStateBase sesion)
        {
            var carrito = sesion[Clave] as List<CarritoItemViewModel>;
            if (carrito == null)
            {
                carrito = new List<CarritoItemViewModel>();
                sesion[Clave] = carrito;
            }
            return carrito;
        }

        public static int TotalEntradas(HttpSessionStateBase sesion)
        {
            var carrito = sesion == null ? null : sesion[Clave] as List<CarritoItemViewModel>;
            return carrito == null ? 0 : carrito.Sum(x => x.Cantidad);
        }

        public static void Limpiar(HttpSessionStateBase sesion)
        {
            sesion[Clave] = null;
        }
    }
}
