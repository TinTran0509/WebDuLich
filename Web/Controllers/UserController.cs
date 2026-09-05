using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /Login/
        private readonly IUserRepository _userRepository = new UserRepository();
        private WebDuLichEntities dbcontext = new WebDuLichEntities();
        public ActionResult LogIn()
        {
            if (User != null) return RedirectToAction("Index", "Home");
            return View();
        }
         

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}