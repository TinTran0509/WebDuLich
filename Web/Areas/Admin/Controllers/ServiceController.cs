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
    public class ServiceController : BaseController
    {

        INewsRepository newsRepository = new NewsRepository();
        ICategoryRepository categoryRepository = new CategoryRepository();
        // GET: Service
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(string keyWord, int pageIndex)
        {
            var model = newsRepository.GetAll().Where(x => x.Type == 2); 
            var totalAdv = model.Count();
            model = model.Skip((pageIndex - 1) * 20).Take(20).OrderByDescending(x => x.CreatedDate).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Service/ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)totalAdv / 20)),
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        [HttpGet]
        public ActionResult Add()
        { 
            var categories = new List<Category>();
            var category = categoryRepository.GetAll().FirstOrDefault(x => x.LinkSeo.Equals("dich-vu"));
            if(category != null)
            {
                categories.Add(category);
                var lstChild1 = categoryRepository.GetAll().Where(g => g.ParentID == category.ID).OrderBy(g => g.Ordering).ToList();
                if (lstChild1.Count > 0)
                {
                    foreach (var tblCate in lstChild1)
                    {
                        tblCate.DisplayOrder = tblCate.Ordering + "";
                        categories.Add(tblCate);
                        var lstChild = categoryRepository.GetAll().Where(g => g.ParentID == tblCate.ID).OrderBy(g => g.Ordering).ToList();
                        if (lstChild.Count > 0)
                        {
                            foreach (var item in lstChild)
                            {
                                item.DisplayOrder = "&nbsp&nbsp" + tblCate.Ordering + "." + item.Ordering;
                                categories.Add(item);
                            }
                        }
                    }
                }
            } 
            TempData["Categories"] = categories;
            return View();
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(News model,string close)
        {
            try
            {
                var news = newsRepository.GetAll()
                 .Where(x => x.Type == 2 && x.MetaTitle.Trim().Equals(model.MetaTitle.Trim()));
                if (news.Any())
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tiêu đề dịch vụ đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh",
                    }, JsonRequestBehavior.AllowGet);
                }
             
                if (string.IsNullOrEmpty(model.Contents))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm nội dung dịch vụ",
                    }, JsonRequestBehavior.AllowGet);
                }
                model.Type = 2; 
                model.CreatedBy =  User.ID;
                newsRepository.Add(model);
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
            var categories = new List<Category>();
            var category = categoryRepository.GetAll().FirstOrDefault(x => x.LinkSeo.Equals("dich-vu"));
            if (category != null)
            {
                categories.Add(category);
                var lstChild1 = categoryRepository.GetAll().Where(g => g.ParentID == category.ID).OrderBy(g => g.Ordering).ToList();
                if (lstChild1.Count > 0)
                {
                    foreach (var tblCate in lstChild1)
                    {
                        tblCate.DisplayOrder = tblCate.Ordering + "";
                        categories.Add(tblCate);
                        var lstChild = categoryRepository.GetAll().Where(g => g.ParentID == tblCate.ID).OrderBy(g => g.Ordering).ToList();
                        if (lstChild.Count > 0)
                        {
                            foreach (var item in lstChild)
                            {
                                item.DisplayOrder = "&nbsp&nbsp" + tblCate.Ordering + "." + item.Ordering;
                                categories.Add(item);
                            }
                        }
                    }
                }
            }
            TempData["Categories"] = categories;
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
                var news = newsRepository.GetAll()
                   .Where(x => x.CategoryId == model.CategoryId && x.MetaTitle.Trim().Equals(model.MetaTitle.Trim())).FirstOrDefault();
                if (news != null && news.ID != model.ID)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tiêu đề dịch vụ đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh",
                    }, JsonRequestBehavior.AllowGet);
                } 
               
                if (string.IsNullOrEmpty(model.Contents))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Vui lòng thêm nội dung dịch vụ",
                    }, JsonRequestBehavior.AllowGet);
                }
                model.ModifiedBy = User.ID;
                newsRepository.Edit(model);
                return Json(new {
                    IsSuccess = true,
                    Messenger = "Cập nhật dịch vụ thành công",
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