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
    public class LanguageController : BaseController
    {
        readonly ILanguageRepository languageRepository= new LanguageRepository();  
        //
    
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
            var model = languageRepository.GetAll().ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Language/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Language/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(tbl_Languages obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm mã",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.LangName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm tên",
                    }, JsonRequestBehavior.AllowGet);
                }
                obj.LangCode = obj.LangCode.ToUpper();
                tbl_Languages languages = languageRepository.GetAll().FirstOrDefault(x=>x.LangCode.Equals(obj.LangCode));
                if(languages != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Mã đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                languageRepository.Add(obj);
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
            var objSlideImages = languageRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Language/_Edit.cshtml", objSlideImages), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(tbl_Languages obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm tên",
                    }, JsonRequestBehavior.AllowGet);
                }
                languageRepository.Edit(obj);
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
            var obj = languageRepository.Find(id); 
            languageRepository.Edit(obj);
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
                var obj = languageRepository.Find(id);
                languageRepository.Delete(id);
               
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
                    languageRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} ngôn ngữ", count),
            }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult LanguagePartial()
        {
            var language = languageRepository.GetAll();
            return PartialView(language);
        }
    }
}
