using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class MenuController : BaseController
    {
        readonly IMenuRepository menuRepository = new MenuRepository();
        readonly IMenuTransRepository menuTransRepository = new MenuTransRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        // 

        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            TempData["Languages"] = languageRepository.GetByActive().ToList();
            return View();
        }
        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(string langCode, int page)
        {  
            TempData["LangCode"] = langCode;
            TempData.Keep("LangCode");
            var menus = new List<MenuTranModel>();
            var lstMenus = menuTransRepository.GetByLangCode(langCode).ToList();
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
                viewContent = RenderViewToString("~/Areas/Admin/Views/Menu/_ListData.cshtml", menus),
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        [HttpGet]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            tbl_Languages tbl_Language = languageRepository.GetByActive().FirstOrDefault(x=>x.IsDefault);
            TempData["Menus"] = menuRepository.GetByLangCode(tbl_Language.LangCode);
            List<MenuLanguageViewModel> menuLanguageViewModels = new List<MenuLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                MenuLanguageViewModel menuLanguageViewModel = new MenuLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                menuLanguageViewModels.Add(menuLanguageViewModel);
            }
            var model = new MenuCreateViewModel
            {
                Languages = menuLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(MenuCreateViewModel model)
        {
            try
            {
                List<MenuTran> lstMenuTrans = menuTransRepository.GetAll().Where(x=>x.MenuID == model.MenuID).ToList();
                 
                List<MenuTran> menuTrans = new List<MenuTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    int parentId = 0;
                    if(lstMenuTrans.Any())
                    {
                        MenuTran menuTran = lstMenuTrans.FirstOrDefault(m=>m.LangCode == lang.LangCode);
                        parentId = menuTran != null ? menuTran.ID : 0;
                    }
                    MenuTran menuTranAdd = new MenuTran
                    {
                        ParentID = parentId,
                        Name = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Ordering = model.Ordering
                    };
                    menuTrans.Add(menuTranAdd);
                }

                Menu menu = new Menu
                {
                    Image = model.Image,
                    CreatedDate = DateTime.Now
                };

                int id = menuRepository.Add(menu);
                foreach (var item in menuTrans)
                {
                    item.MenuID = id; 
                    menuTransRepository.Create(item);
                } 
                
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
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            tbl_Languages tbl_Language = tbl_Languages.FirstOrDefault(x => x.IsDefault);
            TempData["Menus"] = menuRepository.GetByLangCode(tbl_Language.LangCode);

            List<MenuTran> lstMenuTrans = menuTransRepository.GetAll().Where(x => x.MenuID == id).ToList();
             
            MenuTran menuTran = lstMenuTrans.FirstOrDefault(m=>m.LangCode == tbl_Language.LangCode);
             
            int ordering = menuTran != null ? (int)menuTran.Ordering : 0;
            int parentId = menuTran != null ? (int)menuTran.ParentID : 0;

            List <MenuLanguageViewModel> menuLanguageViewModels = new List<MenuLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                MenuTran menuTran_Edit = lstMenuTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);
              
                if(menuTran_Edit != null)
                {
                    MenuLanguageViewModel menuLanguageViewModel = new MenuLanguageViewModel
                    {
                        ID = menuTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = menuTran_Edit.Name
                    };
                    menuLanguageViewModels.Add(menuLanguageViewModel);
                } 
            }
            var model = new MenuCreateViewModel
            {
                MenuID = id,
                ParentID = parentId,
                Ordering = ordering, 
                Languages = menuLanguageViewModels
            };
            return View(model);
        }  

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(MenuCreateViewModel model)
        {
            try
            {
                List<MenuTran> lstMenuTrans = menuTransRepository.GetAll().Where(x => x.MenuID == model.MenuID).ToList();

                List<MenuTran> menuTrans = new List<MenuTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }
                     
                    MenuTran menuTran = lstMenuTrans.Where(m=>m.LangCode == lang.LangCode).FirstOrDefault();
                    int parentId = menuTran != null ? menuTran.ParentID : 0 ;

                    MenuTran menuTranEdit = new MenuTran
                    {
                        ID = lang.ID,
                        ParentID = parentId,
                        Name = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Ordering = model.Ordering
                    };
                    menuTrans.Add(menuTranEdit);
                } 

                foreach (var item in menuTrans)
                {
                    if (item.ParentID == 0)
                    {
                        item.Level = 1;
                    }
                    else
                    {
                        var parent = menuTransRepository.GetAll().FirstOrDefault(x => x.ID == model.ParentID);
                        item.Level = parent != null ? parent.Level + 1 : 0;
                    }
                    menuTransRepository.Edit(item);
                }

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
                List<MenuTran> menuTranList = menuTransRepository.GetAll().Where(m=>m.MenuID == id).ToList();
                menuRepository.Delete(id, menuTranList);
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

        public ActionResult GetLanguages()
        { 
            var languages = languageRepository.GetByActive();
            return Json(new
            {
                Data = languages
            }, JsonRequestBehavior.AllowGet);
        } 

        public ActionResult GetMenuTransByLangCode(string langCode)
        {
            var menus = new List<MenuTran>();
            var lstMenus = menuTransRepository.GetByLangCode(langCode).ToList();
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
    }
}
