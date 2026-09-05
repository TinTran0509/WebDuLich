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
        readonly IUserAdminRepository _userRepository = new UserAdminRepository();
        readonly INewsRepository _newsRepository = new NewsRepository();  
        //readonly IAccessWebsiteReporitory accessWebsiteReporitory = new AccessWebsiteReporitory();
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
    }
}
