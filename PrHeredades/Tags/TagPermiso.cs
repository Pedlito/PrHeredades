using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PrHeredades.Models;

namespace PrHeredades.Tags
{
    public class TagPermiso : ActionFilterAttribute
    {
        public EnumPermisos permiso { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!Sesion.TienePermiso(this.permiso))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Sesion",
                    action = "IniciarSesion"
                }));
            }
        }
    }
}