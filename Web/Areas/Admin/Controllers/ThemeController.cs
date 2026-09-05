using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class ThemeController : BaseController
    {
        readonly IThemeRepository themeRepository = new ThemeRepository();
        readonly IMenuHomeRepository menuHomeRepository = new MenuHomeRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        //
       
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(string keySearch, int page)
        {
            int totalCount = 0; 
            var model = themeRepository.GetByPage(keySearch, page, 20, out totalCount).ToList(); 
            return Json(new
            { 
                viewContent = RenderViewToString("~/Areas/Admin/Views/Theme/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)totalCount / 20)),
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        { 
            TempData["Languages"] = languageRepository.GetByActive().ToList(); 
            return View();
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(Theme obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.LinkSeo))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng chọn menu",
                    }, JsonRequestBehavior.AllowGet);
                }
                obj.GroupID = Guid.NewGuid().ToString();
                obj.Active = true;
                obj.CreatedDate = DateTime.Now;
                themeRepository.Add(obj);
                
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
            TempData["Languages"] = languageRepository.GetByActive().ToList(); 
            var theme = themeRepository.Find(id);
            if (theme != null)
            {
                TempData["Menus"] = menuHomeRepository.GetByLangCode(theme.LangCode).ToList();
            }
            
            return View(theme);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(Theme obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                themeRepository.Edit(obj);
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
         
        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                themeRepository.Delete(id);
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

        public ActionResult GetMenuTransByLangCode(string langCode)
        {
            var menus = new List<MenuHome>();
            var lstMenus = menuHomeRepository.GetByLangCode(langCode).ToList();
            var lstParents = lstMenus.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var parent in lstParents)
                {
                    menus.Add(parent);
                    var lstChild = lstMenus.Where(g => g.ParentID == parent.ID).OrderBy(g => g.Ordering).ToList();
                    if (lstChild.Count > 0)
                    {
                        foreach (var item in lstChild)
                        {
                            item.Name = "-- " + item.Name;
                            menus.Add(item);
                        }
                    }
                }
            }
            else
            {
                menus.AddRange(lstMenus);
            }

            return Json(new
            {
                Data = menus
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        public ActionResult AddTrans(int id)
        { 
            Theme themeAdd = new Theme();
            Theme theme = themeRepository.Find(id);
            if (theme != null)
            {
                var languages = languageRepository.GetByActive().Where(x => x.LangCode != theme.LangCode).ToList();
                TempData["Languages"] = languages;

                themeAdd.Image = theme.Image;
                themeAdd.ParentID = id;
                themeAdd.GroupID = theme.GroupID;
            }

            return View(themeAdd);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult AddTrans(Theme obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.LinkSeo))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng chọn menu",
                    }, JsonRequestBehavior.AllowGet);
                }
                obj.Active = true;
                obj.CreatedDate = DateTime.Now;
                themeRepository.Add(obj);

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
    }
}
