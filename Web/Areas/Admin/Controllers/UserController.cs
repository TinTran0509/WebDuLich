using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using System.Web.Script.Serialization;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        readonly IUserRepository userRepository = new UserRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        readonly ICountryRepository countryRepository = new CountryRepository();
        readonly IUserTransRepository userTransRepository = new UserTransRepository();

        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
            var languages = languageRepository.GetAll();
            var model = userRepository.GetAll().ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            List<UserModel> users = new List<UserModel>();
            foreach (var item in model)
            {
                tbl_Languages language = languages.FirstOrDefault(x=>x.ID == item.LangID);
                UserModel user = new UserModel
                {
                    ID = item.ID,
                    Photo = item.Photo,
                    FullName = item.FullName,
                    Email = item.Email,
                    Phone = item.Phone,
                    Active = item.Active,
                    LangName = language != null ? language.LangName : "",
                };
                users.Add(user);
            }
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/User/_ListData.cshtml", users),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            //TempData["Countries"] = countryRepository.GetAll().ToList();
            TempData["Languages"] = languageRepository.GetByActive().ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/User/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(tbl_User obj)
        {
            try
            {
                if (obj.CountryID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn quốc gia",
                    }, JsonRequestBehavior.AllowGet);
                }
                obj.CreatedDate = DateTime.Now;
                if (string.IsNullOrEmpty(obj.FullName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm tên",
                    }, JsonRequestBehavior.AllowGet);
                }
                tbl_User user = userRepository.GetAll().FirstOrDefault(x=>x.FullName.Equals(obj.FullName));
                if(user != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                userRepository.Add(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int id)
        {
            //TempData["Countries"] = countryRepository.GetAll().ToList();
            TempData["Languages"] = languageRepository.GetByActive().ToList();
            var user = userRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/User/_Edit.cshtml", user), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(tbl_User obj)
        {
            try
            {
                if (obj.LangID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.FullName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm tên",
                    }, JsonRequestBehavior.AllowGet);
                }
                userRepository.Edit(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Cập nhật thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Edit")]
        public ActionResult ChangeStatus(int id)
        {
            var obj = userRepository.Find(id);
            userRepository.Edit(obj);
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Thay đổi trạng thái thành công",
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var obj = userRepository.Find(id);
                userRepository.Delete(id);
               
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa thành công",
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
                    userRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} bản ghi", count),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Translate(int id)
        {
            var userTrans = userTransRepository.GetByUserID(id).ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/User/_Translate.cshtml", userTrans), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLanguages()
        {
            var languages = languageRepository.GetByActive();
            return Json(new
            {
                Data = languages
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveTrans(string menutrans)
        {
            try
            {
                UserTran menuTran = new UserTran();
                JavaScriptSerializer json = new JavaScriptSerializer();
                if (!string.IsNullOrEmpty(menutrans))
                {
                    menuTran = json.Deserialize<UserTran>(menutrans);
                }

                int id = userTransRepository.Create(menuTran);
                return Json(new
                {
                    IsSuccess = true,
                    Id = id,
                    Messenger = "Thêm mới bản dịch thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Thêm mới bản dịch thất bại",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult DeleteUserTrans(int id)
        {
            try
            {
                userTransRepository.Delete(id);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa thành công",
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
