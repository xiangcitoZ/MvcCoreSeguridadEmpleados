using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MvcCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute,
        IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //NOS DA IGUAL QUIEN SE HA VALIDADO POR AHORA
            var user = context.HttpContext.User;
            //NECESITAMOS EL CONTROLLER Y NECESITAMOS EL ACTION
            string controller =
                context.RouteData.Values["controller"].ToString();
            string action =
                context.RouteData.Values["action"].ToString();
            //LA INFORMACION ESTARA DENTRO DE TEMPDATA
            //NECESITAMOS EL PROVEEDOR DE TEMPDATA DE LA APP
            //NUESTRO PROVEEDOR ESTA INYECTADO Y NECESITAMOS 
            //RECUPERAR UN OBJETO INYECTADO
            ITempDataProvider provider =
                context.HttpContext.RequestServices
                .GetService<ITempDataProvider>();
            //A PARTIR DEL PROVEEDOR, DEBEMOS RECUPERAR TEMPDATA
            //QUE UTILIZA NUESTRA APP
            var TempData = provider.LoadTempData(context.HttpContext);
            TempData["controller"] = controller;
            TempData["action"] = action;
            //ALMACENAMOS NUESTRO TEMPDATA DENTRO DE LA APP
            provider.SaveTempData(context.HttpContext, TempData);
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRoute("Managed", "LogIn");
            }
            else
            {
                //NOS INTERESA SABER MAS CARACTERISTICAS DEL USUARIO
                if (user.IsInRole("PRESIDENTE") == false
                    && user.IsInRole("DIRECTOR") == false
                    && user.IsInRole("ANALISTA") == false)
                {
                    context.Result =
                        this.GetRoute("Managed", "ErrorAcceso");
                }
            }
        }


        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(new { controller =controller,
                action = action });
            RedirectToRouteResult result
                = new RedirectToRouteResult(ruta);
            return result;
        }

    }
}
