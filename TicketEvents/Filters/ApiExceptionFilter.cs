using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace TicketEvents.Filters
{
    /// <summary>
    /// Filtro global de la Web API (Unidad 6): manejo centralizado de excepciones.
    /// Los controladores API no necesitan try/catch; toda excepción se traduce aquí
    /// a una respuesta HTTP con un mensaje claro en el formato { "Message": "..." }.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;

            if (ex is DbUpdateConcurrencyException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Conflict,
                    "Otro usuario modificó la información al mismo tiempo. Intenta de nuevo.");
                return;
            }

            if (ex is InvalidOperationException || ex is ArgumentException)
            {
                // Errores de negocio (aforo insuficiente, reseña no permitida, etc.)
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                return;
            }

            Trace.TraceError("[API] {0} {1}: {2}", context.Request.Method, context.Request.RequestUri, ex);
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                "Ocurrió un error inesperado. Intenta de nuevo más tarde.");
        }
    }
}
