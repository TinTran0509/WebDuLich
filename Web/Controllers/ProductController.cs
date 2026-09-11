using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Model;
using Web.Model.CustomModel;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Controllers
{
    public class ProductController : BaseController
    {
        readonly  IProductRepository productRepository = new ProductRepository();
        readonly ILocationRepository locationRepository = new LocationRepository();
        // GET: News
        public ActionResult Index(string linkseo)
        { 
            return View();
        } 
        
        public ActionResult Detail(string linkseo)
        {
            ProductModel model = productRepository.GetByLinkSeo(linkseo);
            if(model != null)
            {
                if(model.Type == 1)
                    ViewBag.TourType = Resources.Language.Group;
                else
                    ViewBag.TourType = Resources.Language.Private;

                var locationViewModels = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode);

                TempData["Locations"] = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode);

                ViewBag.LangName = model.LangName;

                TempData["Relates"] = productRepository.GetRelate(model.MenuID, model.LangCode, 3);
            }
            ViewBag.Destination = Resources.Language.Destination;
            ViewBag.Duration = Resources.Language.Duration;
            ViewBag.AverageSize = Resources.Language.AverageSize;
            ViewBag.Price = Resources.Language.Price;
            ViewBag.GuideLang = Resources.Language.GuideLang;
            ViewBag.ProductCode = Resources.Language.ProductCode;
            ViewBag.TourTypeDes = Resources.Language.TourType;
            ViewBag.HomePage = Resources.Language.HomePage;
            ViewBag.SelectDate = Resources.Language.SelectDate;
            ViewBag.YeuCauBaoGia = Resources.Language.YeuCauBaoGia;
            ViewBag.ThemLichTrinh = Resources.Language.ThemLichTrinh;
            ViewBag.Days = Resources.Language.Days;
            ViewBag.Nigths = Resources.Language.Nigths;
            ViewBag.From = Resources.Language.From;
            ViewBag.Detail = Resources.Language.Detail;

            return View(model);
        }
    }
}