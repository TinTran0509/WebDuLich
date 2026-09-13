using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class BookingController : BaseController
    {
        private readonly IProductTransRepository productTransRepository = new ProductTransRepository();
        private readonly ILocationRepository locationRepository = new LocationRepository();
        private readonly IWordRepository wordRepository = new WordRepository();
        // GET: News
        public ActionResult Index()
        { 
            string productId = Request.QueryString["tour_id"];
            int id = 0;
            int.TryParse(productId, out id);

            ProductModel model = productTransRepository.GetByProductTransId(id);

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(model.Culture);

            ViewBag.BookingNow = Resources.Language.BookingNow;
            ViewBag.HomePage = Resources.Language.HomePage;
            ViewBag.ThankYouRequest = Resources.Language.ThankYouRequest;
            ViewBag.Note = Resources.Language.Note;//Passenger
            ViewBag.ArrivalDate = Resources.Language.ArrivalDate;
            ViewBag.Days = Resources.Language.Days;
            ViewBag.Nigths = Resources.Language.Nigths;
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
            ViewBag.DanhXung = Resources.Language.DanhXung;
            ViewBag.Name = Resources.Language.Name; //LastName
            ViewBag.LastName = Resources.Language.LastName;
            ViewBag.PlaceTel = Resources.Language.PlaceTel;
            ViewBag.NgonNguUuTien = Resources.Language.NgonNguUuTien;
            ViewBag.DepartureCity = Resources.Language.DepartureCity;
            ViewBag.NotesAnd = Resources.Language.NotesAnd;
            ViewBag.PlaceNotes = Resources.Language.PlaceNotes;
            ViewBag.YCBaoGia = Resources.Language.YCBaoGia;

            string sLocation = string.Empty;
            string startPoint = string.Empty;
            List<LocationViewModel> locations = locationRepository.GetByLocationIDs(model.LocationID, model.LangCode).ToList();
            foreach (var item in locations)
            {
                if(sLocation == string.Empty)
                {
                    startPoint = item.Name;
                    sLocation += item.Name;
                }
                else
                {
                    sLocation += "," + item.Name;
                } 
            }
            if (model.Type == 1)
                ViewBag.TourType = Resources.Language.Group;
            else
                ViewBag.TourType = Resources.Language.Private;
            ViewBag.StartPoint = startPoint;
            ViewBag.Location = sLocation;
            return View(model);
        }  
    }
}