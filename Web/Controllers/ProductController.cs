using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
        private readonly IProductTransRepository productTransRepository = new ProductTransRepository();
        private readonly ILocationRepository locationRepository = new LocationRepository();
        private readonly IWordRepository wordRepository = new WordRepository();
        private readonly IMenuTransRepository menuTransRepository = new MenuTransRepository();
        private readonly IUserRepository userRepository = new UserRepository();  

        public ActionResult Index(string linkseo)
        {
            string langCode = string.Empty;
            MenuTranModel menuTranModel = menuTransRepository.GetByLinkSeo(linkseo); 

            if(menuTranModel != null)
            {
                ViewBag.Title = menuTranModel.Title;

                langCode = menuTranModel.LangCode;

                TempData["GroupTour"] = productTransRepository.GetByType(1, menuTranModel.LangCode, menuTranModel.ID, 9);

                TempData["PrivateTour"] = productTransRepository.GetByType(2, menuTranModel.LangCode, menuTranModel.ID, 3);

                TempData["UserModel"] = userRepository.GetAllByLangCode(langCode);

                TempData["Locations"] = locationRepository.GetLocationTranByCoutryName(langCode, menuTranModel.Name);

                ViewBag.Description = menuTranModel.Description;
                ViewBag.GroupTrip = wordRepository.GetValueByKey("GroupTrip", langCode);
                ViewBag.CustomizedTrips = wordRepository.GetValueByKey("CustomizedTrips", langCode);
                ViewBag.Price = Resources.Language.Price;
                ViewBag.Days = Resources.Language.Days;
                ViewBag.Nigths = Resources.Language.Nigths;
                ViewBag.Detail = Resources.Language.Detail;
                ViewBag.SeeMore = Resources.Language.SeeMore;
                ViewBag.TravelDestinations = wordRepository.GetValueByKey("TravelDestinations", langCode);
                ViewBag.OurSpecialists = wordRepository.GetValueByKey("OurSpecialists", langCode);
                ViewBag.WhatSee = wordRepository.GetValueByKey("WhatSee", langCode);
                ViewBag.Media = wordRepository.GetValueByKey("Gallery", langCode);
                ViewBag.HomePage = Resources.Language.HomePage;
                ViewBag.Summary = Resources.Language.Summary;
                ViewBag.Itinerary = Resources.Language.Itinerary;
                ViewBag.Highlights = Resources.Language.Highlights;
                ViewBag.Gallery = Resources.Language.Gallery;
                ViewBag.OurExpert = Resources.Language.OurExpert;
            }
            else
            {
                ViewBag.Title = Resources.Language.TitlePage;
                ViewBag.Description = Resources.Language.Description;
            }
             
            return View(menuTranModel);
        } 
        
        public ActionResult Detail(string linkseo)
        {
            ProductModel model = productTransRepository.GetByLinkSeo(linkseo);
            if(model != null)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(model.Culture);

                if (model.Type == 1)
                    ViewBag.TourType = Resources.Language.Group;
                else
                    ViewBag.TourType = Resources.Language.Private;

                var locationViewModels = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode);

                TempData["Locations"] = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode);

                ViewBag.LangName = model.LangName;

                TempData["Relates"] = productTransRepository.GetRelate(model.ProductID, model.MenuID, model.LangCode, 3);

                ViewBag.EstimatedPrice = wordRepository.GetValueByKey("EstimatedPrice", model.LangCode);

                var basePrice = model.Price;
                var year = DateTime.Now.Year; 

                DateTime startDate = new DateTime(year, 1, 1);
                DateTime endDate = new DateTime(year, 12, 31);
                 
                Dictionary<string, string> data = new Dictionary<string, string>();

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {  
                    decimal price = (decimal)basePrice;

                    if (date.DayOfWeek == DayOfWeek.Saturday ||
                        date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        price *= 1.05m; // tăng 5%
                    }

                    data.Add(date.ToString("yyyy-MM-dd"), "USD " + price.ToString("0.##")); 
                }

                string jsonPrice = JsonConvert.SerializeObject(data); 

                ViewBag.DatePrice = jsonPrice;
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