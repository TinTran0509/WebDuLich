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
        // GET: News
        public ActionResult Index()
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
            }
            ViewBag.Destination = Resources.Language.Destination;
            ViewBag.Duration = Resources.Language.Duration;
            ViewBag.AverageSize = Resources.Language.AverageSize;
            ViewBag.Price = Resources.Language.Price;
            return View(model);
        }
    }
}