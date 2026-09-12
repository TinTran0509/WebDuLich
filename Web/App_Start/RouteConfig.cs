using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("TimKiem", "search", new { controller = "Category", action = "Search" }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("GioiThieu", "gioi-thieu", new { controller = "About", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("Booking", "booking-tour", new { controller = "Booking", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("News", "tin-tuc", new { controller = "News", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("LienHe", "lien-he", new { controller = "Contact", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("ListBlog", "danhsachblog", new { controller = "Blog", action = "LoadData", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("ListService", "danhsachdichvu", new { controller = "Category", action = "ListCate" }, namespaces: new[] { "Web.Controllers" }); 
 
            routes.MapRoute("NewsDetail", "tin-tuc/{linkseo}", new { controller = "News", action = "Detail", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("ProductList", "{linkseo}", new { controller = "Product", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("ProductDetail", "tour/{linkseo}", new { controller = "Product", action = "Detail", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("dangnhap", "dang-nhap.html", new { controller = "Login", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute("TrangChu", "{langCode}/index.html", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Web.Controllers" }
            );
        }
    }
}