using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
        private readonly IProductRepository productRepository = new ProductRepository();
        private readonly IProductTransRepository productTransRepository = new ProductTransRepository();
        private readonly ILocationRepository locationRepository = new LocationRepository();
        private readonly IWordRepository wordRepository = new WordRepository();
        private readonly IMenuTransRepository menuTransRepository = new MenuTransRepository();
        private readonly IUserRepository userRepository = new UserRepository();
        private readonly IPackageRepository _packageRepository = new PackageRepository();

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
                ViewBag.Title = wordRepository.GetValueByKey("Title", model.LangCode);
                 
                List<Itinerary> itineraries = productTransRepository.GetItineraryByProductIDAndLangCode(model.ProductID, model.LangCode).ToList();
                TempData["Itineraries"] = itineraries;

                List<Package_Price> package_Prices = _packageRepository.GetAllPackagePrice().Where(x => x.ProductID == model.ProductID).ToList();

                string json = JsonConvert.SerializeObject(package_Prices);

                ViewBag.JsonPackage = json;

                List<ImportantNote> importantNotes = productRepository.GetAllImportantNote().Where(x => x.ProductID == model.ProductID && x.LangCode == model.LangCode).ToList();

                if (importantNotes.Any())
                {
                    ImportantNote inc = importantNotes.FirstOrDefault(x => x.KeyNote == "INCLUDED");
                    ViewBag.Includeds = inc != null ? inc.Contents : "";
                    ImportantNote exc = importantNotes.FirstOrDefault(x => x.KeyNote == "EXCLUDED");
                    ViewBag.Excludeds = exc != null ? exc.Contents : "";
                    ImportantNote imp = importantNotes.FirstOrDefault(x => x.KeyNote == "IMPORTANT");
                    ViewBag.ImportantNots = imp != null ? imp.Contents : "";
                    ImportantNote hig = importantNotes.FirstOrDefault(x => x.KeyNote == "HIGHLIGHTS");
                    ViewBag.HighlightsDetail = hig != null ? hig.Contents : "";
                }
                else
                {
                    ViewBag.Includeds = wordRepository.GetValueByKey("Included", model.LangCode);
                    ViewBag.Excludeds = wordRepository.GetValueByKey("Excluded", model.LangCode);
                    ViewBag.ImportantNots = wordRepository.GetValueByKey("ImportantNot", model.LangCode);
                    ViewBag.HighlightsDetail = wordRepository.GetValueByKey("Highlights", model.LangCode);
                }
            }
            ViewBag.Destination = Resources.Language.Destination;
            ViewBag.Duration = Resources.Language.Duration;
            ViewBag.AverageSize = Resources.Language.AverageSize;
            ViewBag.Price = Resources.Language.Price;
            ViewBag.GuideLang = Resources.Language.GuideLang;
            ViewBag.ProductCode = Resources.Language.ProductCode;
            ViewBag.TourTypeDes = Resources.Language.TourType;
            ViewBag.Transport = Resources.Language.Transport;
            ViewBag.Meals = Resources.Language.Meals;
            ViewBag.Private = Resources.Language.Private;
            ViewBag.Sharing = Resources.Language.Sharing;
            ViewBag.Destinations = Resources.Language.Destinations;
            ViewBag.Included = Resources.Language.Included;
            ViewBag.Excluded = Resources.Language.Excluded;
            ViewBag.ImportantNot = Resources.Language.ImportantNot;
            ViewBag.HomePage = Resources.Language.HomePage;
            ViewBag.SelectDate = Resources.Language.SelectDate;
            ViewBag.YeuCauBaoGia = Resources.Language.YeuCauBaoGia;
            ViewBag.ThemLichTrinh = Resources.Language.ThemLichTrinh;
            ViewBag.Days = Resources.Language.Days;
            ViewBag.Nigths = Resources.Language.Nigths;
            ViewBag.From = Resources.Language.From;
            ViewBag.Detail = Resources.Language.Detail; 
            ViewBag.Star = Resources.Language.Star;
            ViewBag.Package = Resources.Language.Package;
            ViewBag.Plus = Resources.Language.Plus; 
            ViewBag.Summary = Resources.Language.Summary;
            ViewBag.Itinerary = Resources.Language.Itinerary;
            ViewBag.Highlights = Resources.Language.Highlights;
            ViewBag.Package = Resources.Language.Package;
            ViewBag.Star = Resources.Language.Star;
            ViewBag.Plus = Resources.Language.Plus;
            ViewBag.NumberPas = Resources.Language.NumberPas;
            ViewBag.ArrivalDate = Resources.Language.ArrivalDate;
            ViewBag.TongUocTinh = Resources.Language.TongUocTinh;
            ViewBag.DanhXung = Resources.Language.DanhXung;
            ViewBag.Name = Resources.Language.Name; //LastName
            ViewBag.LastName = Resources.Language.LastName;
            ViewBag.PlaceTel = Resources.Language.PlaceTel;
            ViewBag.NgonNguUuTien = Resources.Language.NgonNguUuTien;
            ViewBag.DepartureCity = Resources.Language.DepartureCity;
            ViewBag.NotesAnd = Resources.Language.NotesAnd;
            ViewBag.PlaceNotes = Resources.Language.PlaceNotes;
            ViewBag.YCBaoGia = Resources.Language.YCBaoGia;

            ViewBag.MessageSelectPackage = Resources.Language.MessageSelectPack;
            ViewBag.MessageSelectDate = Resources.Language.MessageSelectDate;

            return View(model);
        }

        [HttpPost]
        public ActionResult Booking(Booking booking)
        {
            try
            {
                IBookingRepository bookingRepository = new BookingRepository();
                booking.Status = 1;
                booking.CreatedDate = DateTime.Now;
                
                SentMail sentMail = new SentMail();
                sentMail.Status = 0;
                sentMail.CreatedDate = DateTime.Now;
                sentMail.Subject = "Thông báo khách hàng đặt tour"; 

                bookingRepository.Create(booking, sentMail);

                return Json(new
                {
                    success = true,
                    message = Resources.Language.BookingSuccess,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = Resources.Language.BookingFailed,
                }, JsonRequestBehavior.AllowGet);

            }  
        }
    }
}