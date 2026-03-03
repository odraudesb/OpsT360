using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MiWebApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        //errores de registros de almacenamientos de cache, autorizaciones.
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                List<string> errorList = new List<string>();
                foreach (var err in actionContext.ModelState.Values)
                {
                    foreach (var er in err.Errors)
                    {
                        errorList.Add(er.ErrorMessage);
                    }
                }
                var message = string.Format("Datos inválidos \r\n{0} ", String.Join("\n\r", errorList.ToArray()));
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
        }
    }
}