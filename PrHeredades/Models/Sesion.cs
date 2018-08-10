using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PrHeredades.Models
{
    public class Sesion
    {
        public static bool Existe()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
        public static void Cerrar()
        {
            FormsAuthentication.SignOut();
        }

        public static int ObtenerCodigo()
        {
            Usuario user = new Usuario();
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user = JsonConvert.DeserializeObject<Usuario>(ticket.UserData);
                }
            }
            return user.codUsuario;
        }

        public static string ObtenerNombre()
        {
            Usuario user = new Usuario();
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user = JsonConvert.DeserializeObject<Usuario>(ticket.UserData);
                }
            }
            return user.nombre;
        }

        public static void Iniciar(Usuario usuario)
        {
            bool persist = true;
            var cookie = FormsAuthentication.GetAuthCookie("usuario", persist);

            cookie.Name = FormsAuthentication.FormsCookieName;
            cookie.Expires = DateTime.Now.AddHours(1);

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, JsonConvert.SerializeObject(usuario));

            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static bool TienePermiso(EnumPermisos valor)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            int codUsuario = ObtenerCodigo();
            int codRol = (from t in db.tbUsuario where t.codUsuario == codUsuario select t.codRol).SingleOrDefault();
            return (from t in db.tbRolPermiso where t.codRol == codRol && t.codPermiso == (int)valor select t.estado).SingleOrDefault().Value;
        }
    }

    public enum EnumPermisos
    {
        Catalogos = 1,
        Usuarios = 2,
        Inventario = 3,
        Roles = 4
    }
}