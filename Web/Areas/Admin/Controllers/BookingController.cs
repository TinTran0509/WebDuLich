using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class BookingController : BaseController
    {
       private readonly IBookingRepository bookingRepository = new BookingRepository();
        private readonly IUserAdminRepository _userAdminRepository = new UserAdminRepository();
        //
        // GET: /Admin/Booking/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
      
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Booking/_ListData.cshtml", null),
                totalPages = Math.Ceiling(((double)2 / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }
          
        public ActionResult Detail(int id)
        {
            Booking booking =  bookingRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Booking/_Detail.cshtml", booking), JsonRequestBehavior.AllowGet);
        } 
    }
}
