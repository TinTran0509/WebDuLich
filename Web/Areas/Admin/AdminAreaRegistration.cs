using System.Web.Mvc;
using Web.App_Start;

namespace Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.Routes.Add("Admin_Default", new SubDomainRoute(
            //    "{controller}/{action}/{id}",
            //    new { area = "Admin", controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    new[] { typeof(Controllers.HomeController).Namespace } // Namespaces defaults
            //));
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
