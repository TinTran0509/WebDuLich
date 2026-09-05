using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using PagedList;
using Web.BaseSecurity;
using Web.Controllers;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Web.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/
        readonly IGroupUserRepository _groupuserRepository = new GroupUserRepository();
        readonly IUserAdminRepository _userRepository = new UserAdminRepository();
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (User != null) return RedirectToAction("Index", "Home");
            return View("/Areas/Admin/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public ActionResult Login(UserAdmin obj)
        {
            if (string.IsNullOrEmpty(obj.UserName) || string.IsNullOrWhiteSpace(obj.Password))
            {
                Session["Messenger"] = new Notified { Value = EnumNotifield.Error, Messenger = "Bạn chưa nhập tên đăng nhập hoặc mật khẩu nhập vào không đúng!" };
                return View("/Areas/Admin/Views/Account/Login.cshtml");
            }
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Password == HelperEncryptor.Md5Hash(obj.Password) && u.UserName == obj.UserName);
            if (user != null)
            {
                _userRepository.Edit(user);
                if (user.Active == false)
                {
                    Session["Messenger"] = new Notified { Value = EnumNotifield.Error, Messenger = "Tài khoản này chưa được kích hoạt!" };
                    return View("/Areas/Admin/Views/Account/Login.cshtml");
                }
                var serializeModel = new CustomPrincipalSerializeModel
                {
                    ID = user.ID,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    GroupUser = user.GroupUserID,
                    UserType = user.UserType,
                    isAdmin = user.isAdmin,
                };
                var serializer = new JavaScriptSerializer();

                var userData = serializer.Serialize(serializeModel);

                var authTicket = new FormsAuthenticationTicket(
                    1,
                    obj.UserName,
                    DateTime.Now,
                    DateTime.Now.AddHours(1),
                    false,
                    userData);

                var encTicket = FormsAuthentication.Encrypt(authTicket);
                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                faCookie.Name = "ADMIN_COOKIES";
                Response.Cookies.Add(faCookie);
               
            }
            else
            {
                Session["Messenger"] = new Notified { Value = EnumNotifield.Error, Messenger = "Bạn chưa nhập tên đăng nhập hoặc mật khẩu nhập vào không đúng!" };

                return View("/Areas/Admin/Views/Account/Login.cshtml");
            }
            var preUrl = (Uri)Session["returnUrl"];
            if (preUrl != null)
            {
                Session["returnUrl"] = null;
                return Redirect(preUrl.ToString());
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [Authorize(Roles = "Add")]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "Edit")]
        public ActionResult ChangeStatus(int id)
        {
            var obj = _userRepository.Find(id);
            obj.Active = !obj.Active;
            _userRepository.Edit(obj);
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Thay đổi trạng thái thành công",
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Register(UserAdmin user)
        {
            _userRepository.Add(user);
            var serializeModel = new CustomPrincipalSerializeModel
            {
                ID = user.ID,
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                GroupUser = user.GroupUserID,
                UserType = user.UserType,
                isAdmin = user.isAdmin,
            };

            var serializer = new JavaScriptSerializer();

            var userData = serializer.Serialize(serializeModel);

            var authTicket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.AddYears(1),
                false,
                userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
            return View();
        }
        //GET: /Account/Logout
        public ActionResult Logout()
        {
            //FormsAuthentication.SignOut();
            var httpCookie = Response.Cookies["ADMIN_COOKIES"];
            if (httpCookie != null) httpCookie.Expires = DateTime.Now.AddDays(-1);
           
            return RedirectToAction("Login", "Account");
        }
        #region QUẢN LÝ NGƯỜI DÙNG

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(string Name, int page = 1)
        {
            var lstUser = _userRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Name))
            {
                lstUser = lstUser.Where(g => HelperString.UnsignCharacter(g.FullName.Trim().ToLower()).Contains(HelperString.UnsignCharacter(Name.ToLower().Trim())) || HelperString.UnsignCharacter(g.UserName.Trim().ToLower()).Contains(HelperString.UnsignCharacter(Name.ToLower().Trim()))).ToList();
            }
            var lstLevel = lstUser.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            TempData["NguoiDung"] = lstUser;
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Account/_ListData.cshtml", lstLevel),
                totalPages = Math.Ceiling(((double)lstUser.Count / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }
     
        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            var lstGroupUser = _groupuserRepository.GetAll().ToList();
            TempData["lstGroupUser"] = lstGroupUser;
            return Json(RenderViewToString("~/Areas/Admin/Views/Account/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Add")]
        public ActionResult Add(UserAdmin obj, int[] arrquyTrinhXuatBanId)
        {
            try
            {
                obj.CreatedDate = DateTime.Now;
                obj.Password = HelperEncryptor.Md5Hash(obj.Password);
                _userRepository.Add(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới người dùng thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới người dùng thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int id)
        {
            var objUser = _userRepository.Find(id);
            var lstGroupUser = _groupuserRepository.GetAll().ToList();
            TempData["lstGroupUser"] = lstGroupUser;
            return Json(RenderViewToString("~/Areas/Admin/Views/Account/_Edit.cshtml", objUser), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(UserAdmin obj, int[] arrquyTrinhXuatBanId, int[] arrPageElementId)
        {
            try
            {
                var objUser = _userRepository.Find(obj.ID);
                objUser.FullName = obj.FullName;
                objUser.UserType = obj.UserType;
                objUser.Active = obj.Active;
                objUser.isAdmin = obj.isAdmin;
                objUser.GroupUserID = obj.GroupUserID;
                _userRepository.Edit(objUser);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật người dùng thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Cập nhật người dùng thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _userRepository.Delete(id);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa danh người dùng thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa người dùng thành công",
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult DeleteAll(string lstid)
        {
            var arrid = lstid.Split(',');
            var count = 0;
            foreach (var item in arrid)
            {
                try
                {
                    _userRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} người dùng", count),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        public ActionResult ResetPassword(int id)
        {
            var objUser = _userRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Account/ResetPassword.cshtml", objUser), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult ResetPassword(int id, string password, string confirmpassword)
        {
            if (password == confirmpassword)
            {
                try
                {
                    var objUser = _userRepository.Find(id);
                    objUser.Password = HelperEncryptor.Md5Hash(password);
                    _userRepository.Edit(objUser);
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Reset mật khẩu thành công",
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Reset mật khẩu thất bại",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Reset mật khẩu thất bại",
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Edit")]
        public ActionResult ChangePass(int id)
        {
            var objUser = _userRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Account/ChangePass.cshtml", objUser), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult ChangePass(int id, string oldpassword, string password, string confirmpassword)
        {
            var objUser = _userRepository.Find(id);
            if (objUser.Password != HelperEncryptor.Md5Hash(oldpassword))
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Mật khẩu cũ nhập vào không đúng",
                }, JsonRequestBehavior.AllowGet);
            }
            if (password == confirmpassword)
            {
                try
                {
                    objUser.Password = HelperEncryptor.Md5Hash(password);
                    _userRepository.Edit(objUser);
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Reset mật khẩu thành công",
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Reset mật khẩu thất bại",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                IsSuccess = false,
                Messenger = "Reset mật khẩu thất bại",
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}