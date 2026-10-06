using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using Web.BaseSecurity;
using Web.Controllers;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;
using CMS.IRepository;
using CMS.Reporitory;

namespace Web.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserAdminRepository _userRepository = new UserAdminRepository();
        private readonly IBookingRepository bookingRepository = new BookingRepository(); 
        // GET: /Admin/Home/
        [Authorize]
        public ActionResult Index()
        {
            if (User == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (User.UserType == (int)EnumHelper.UserType.Binhthuong)
            {
                return RedirectToAction("AccessDenined", "Error");
            }
            // Thông tin đăng nhập User:
            var currentUser = _userRepository.Find(User.ID);
            ViewBag.rowUser = currentUser;  
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(string keySearch, int status, int page = 1)
        {
            int total = 0;
            var model = bookingRepository.GetByPage(keySearch, status, page, 20, out total).ToList();
          
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Home/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Home/_ProductDetail.cshtml", null), JsonRequestBehavior.AllowGet);
        }
    }
}
