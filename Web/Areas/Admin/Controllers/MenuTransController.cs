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
    public class MenuTransController : BaseController
    {
        readonly IMenuTransRepository menuTransRepository = new MenuTransRepository();
        readonly IMenuRepository menuRepository = new MenuRepository();
        //
        // GET: /Admin/HomeMenu/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page)
        {
            var model = menuTransRepository.GetAll().ToList();
            var totalAdv = model.Count();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/MenuTrans/_ListData.cshtml", model),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Index")]
        public ActionResult GetAllByLangCode(string LangCode)
        {
            LangCode = LangCode ?? Webconfig.LangCodeVn;
            var lstHomeMenu = menuTransRepository.GetAll().ToList();
            var lstLevel = Common.CreateLevel(lstHomeMenu.ToList());
            return Json(lstLevel, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            TempData["Menus"] = menuRepository.GetAll().ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/MenuTrans/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(MenuTran obj)
        {
            try
            {
                var menu = menuTransRepository.GetAll().FirstOrDefault(x => x.Name.Trim() == obj.Name.Trim());
                if(menu != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                menuTransRepository.Add(obj);
                
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
            TempData["Menus"] = menuRepository.GetAll().ToList();
            var objHomeMenu = menuTransRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Menu/_Edit.cshtml", objHomeMenu), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(MenuTran obj)
        {
            try
           {
                var munuName = menuTransRepository.GetAll().FirstOrDefault(x => x.Name.Trim() == obj.Name.Trim() && x.ID != obj.ID);
                if(munuName!= null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                menuTransRepository.Edit(obj);
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
                var obj = menuTransRepository.Find(id);
                menuTransRepository.Delete(id);
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
                    // xoa danhh muc
                    menuTransRepository.Delete(Convert.ToInt32(item));
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
    }
}
