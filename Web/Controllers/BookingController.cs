using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
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
    public class BookingController : BaseController
    {
        private readonly  IProductRepository productRepository = new ProductRepository();
        private readonly ILocationRepository locationRepository = new LocationRepository();
        private readonly IWordRepository wordRepository = new WordRepository();
        // GET: News
        public ActionResult Index()
        {
            string productId = Request.QueryString["tour_id"];
            int id = 0;
            int.TryParse(productId, out id);

            ProductModel model = productRepository.GetById(id);

            ViewBag.BookingNow = Resources.Language.BookingNow;
            ViewBag.HomePage = Resources.Language.HomePage;
            ViewBag.ThankYouRequest = Resources.Language.ThankYouRequest;
            ViewBag.Note = Resources.Language.Note;//Passenger
            ViewBag.ArrivalDate = Resources.Language.ArrivalDate;
            ViewBag.Itinerary = Resources.Language.Itinerary;
            ViewBag.Destination = Resources.Language.Destination;
            ViewBag.StartingPoint = Resources.Language.StartingPoint;
            ViewBag.HotelCategory = Resources.Language.HotelCategory;
            ViewBag.LangName = model.LangName;
            ViewBag.Journey = Resources.Language.Journey;
            ViewBag.Passenger = Resources.Language.Passenger;
            ViewBag.UocTinhTrenNguoi = Resources.Language.UocTinhTrenNguoi;
            ViewBag.Adult = Resources.Language.Adult;
            ViewBag.Child = Resources.Language.Child;
            ViewBag.Baby = Resources.Language.Baby;
            ViewBag.Select = Resources.Language.Select;
            ViewBag.Continue = Resources.Language.Continue;
            ViewBag.PhuPhi = Resources.Language.PhuPhi;
            ViewBag.TongUocTinh = Resources.Language.TongUocTinh;
            ViewBag.Title = Resources.Language.Title;
            ViewBag.Name = Resources.Language.Name; //LastName
            ViewBag.LastName = Resources.Language.LastName;
            ViewBag.PlaceTel = Resources.Language.PlaceTel;
            ViewBag.NgonNguUuTien = Resources.Language.NgonNguUuTien;
            ViewBag.DepartureCity = Resources.Language.DepartureCity;
            ViewBag.NotesAnd = Resources.Language.NotesAnd;
            ViewBag.PlaceNotes = Resources.Language.PlaceNotes;
            ViewBag.YCBaoGia = Resources.Language.YCBaoGia;

            string sLocation = string.Empty;
            List<LocationViewModel> locations = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode).ToList();
            foreach (var item in locations)
            {
                sLocation += sLocation == string.Empty ? item.Name : "," + item.Name;
            }
            ViewBag.Location = sLocation;
            return View(model);
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