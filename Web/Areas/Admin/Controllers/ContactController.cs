using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class ContactController : BaseController
    {

        INewsRepository newsRepository = new NewsRepository();
        ICategoryRepository categoryRepository = new CategoryRepository();
        // GET: Contact
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int pageIndex)
        {
            var category = categoryRepository.GetAll().FirstOrDefault();
            var model = category.News;
            var totalAdv = model.Count();
            model = model.Skip((pageIndex - 1) * 20).Take(20).OrderByDescending(x => x.CreatedDate).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Contact/ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)totalAdv / 20)),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var obj = newsRepository.Find(id);
            return View(obj);
        }

        [Authorize(Roles = "Add")]
        [HttpGet]
        public ActionResult Add()
        { 
            return View();
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(News model,string close)
        {
            try
            {
                
                return Json(new
                {
                    Close = close,
                    IsSuccess = true,
                    Messenger = "Thêm mới thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Thêm mới thất bại "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        [HttpGet]
        public ActionResult Edit(int id)
        { 
            var obj = newsRepository.Find(id);
            return View(obj);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(News model)
        {
            try
            { 
                  
                return Json(new {
                    IsSuccess = true,
                    Messenger = "Cập nhật bài viết thành công",
                    JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Messenger = "Cập nhật thất bại", JsonRequestBehavior.AllowGet });
            }
        }

        [Authorize(Roles = "Delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                newsRepository.Delete(id);
                return Json(new { IsSuccess = true, Messenger = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Messenger = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
        } 
    }
}