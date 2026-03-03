using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;


namespace MiWebApi
{
  
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string api = "Api.cgsa";
            try
            { api = actionExecutedContext.Request.RequestUri.LocalPath + actionExecutedContext.ActionContext.ActionDescriptor.ActionName; }
            catch
            {
            }

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
            HttpStatusCode.InternalServerError,
            new
            {
                Title = "Se produjo un error al procesar la solicitud.",
                Message = "Momentáneamente su transacción no puede ser procesada. Favor intente en unos minutos",
                Type = "Error",
                Code = actionExecutedContext.Exception.HResult
            });
            actionExecutedContext.Response.ReasonPhrase = "Se produjo un error al procesar la solicitud.";
        }
    }
}