using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Web.BaseSecurity;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Application["Totaluser"] = 0;
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);  
        }
        protected void Session_Start()
        {
            Application.Lock();
            Application["Totaluser"] = (int)Application["Totaluser"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();
            Application["Totaluser"] = (int)Application["Totaluser"] - 1;
            Application.UnLock();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie;
            if (Request.FilePath.Split('/').Select(g=>g.ToLower()).Contains("admin") || Request.Url.Host.Split('.')[0].ToLower() == "admin")
            {
                authCookie = Request.Cookies["ADMIN_COOKIES"];
            }
            else
            {
                authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            }
            //var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null) return;
            FormsAuthenticationTicket authTicket = null;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception)
            {
                var httpCookie = Response.Cookies["ADMIN_COOKIES"];
                if (httpCookie != null) httpCookie.Expires = DateTime.Now.AddDays(-1);
            }
            var serializer = new JavaScriptSerializer();

            if (authTicket != null)
            {
                var serializeModel = serializer.Deserialize<CustomPrincipalSerializeModel>(authTicket.UserData);
            
                var newUser = new CustomPrincipal(authTicket.Name)
                {
                    ID = serializeModel.ID,
                    Username = serializeModel.Username,
                    Email = serializeModel.Email,
                    UserType = serializeModel.UserType,
                    GroupUser = serializeModel.GroupUser,
                    NoiBo = serializeModel.NoiBo,
                    isAdmin = serializeModel.isAdmin,
                };
                HttpContext.Current.User = newUser;
            }
        }
    }
}