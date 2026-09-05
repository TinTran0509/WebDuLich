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
    public class NewsController : BaseController
    {

        INewsRepository newsRepository = new NewsRepository();
        ICategoryRepository categoryRepository = new CategoryRepository();
        // GET: News
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(string keyWord, int pageIndex)
        {
            var category = categoryRepository.GetAll().Where(x => x.LinkSeo.Equals("tin-tuc")).FirstOrDefault();
            var model = category.News; 
            var totalAdv = model.Count();
            model = model.Skip((pageIndex - 1) * 20).Take(20).OrderByDescending(x => x.CreatedDate).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/News/ListData.cshtml", model),
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
                int categoryId = 0;
                var category = categoryRepository.GetAll().Where(x => x.LinkSeo.Equals("tin-tuc")).FirstOrDefault();
                if (category != null)
                {
                    categoryId = category.ID;
                    var news = newsRepository.GetAll()
                  .Where(x => x.CategoryId == categoryId && x.MetaTitle.Trim().Equals(model.MetaTitle.Trim()));
                    if (news.Any())
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Tiêu đề tin tức đã tồn tại",
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Đã có lỗi xảy ra. Vui lòng thử lại sau",
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
                        Messenger = "Vui lòng thêm nội dung tin tức",
                    }, JsonRequestBehavior.AllowGet);
                }
                model.Type = 1;
                model.CategoryId = categoryId;
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
                        Messenger = "Tiêu đề tin tức đã tồn tại",
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
                        Messenger = "Vui lòng thêm nội dung tin tức",
                    }, JsonRequestBehavior.AllowGet);
                }
                model.ModifiedBy = User.ID;
                newsRepository.Edit(model);
                return Json(new {
                    IsSuccess = true,
                    Messenger = "Cập nhật tin tức thành công",
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