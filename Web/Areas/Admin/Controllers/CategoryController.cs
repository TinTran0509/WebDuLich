using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
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
    public class CategoryController : BaseController
    {
        ICategoryRepository categoryRepository = new CategoryRepository(); 
        INewsRepository newsRepository = new NewsRepository();
        // GET: Menu
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int page)
        {
            var categories = new List<Category>();
            var lstCategories = categoryRepository.GetAll().OrderBy(x=>x.Ordering).ToList();
            var lstParents = lstCategories.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var tblCate in lstParents)
                {
                    tblCate.DisplayOrder = tblCate.Ordering + "";
                    categories.Add(tblCate);
                    var lstChild = lstCategories.Where(g => g.ParentID == tblCate.ID).OrderBy(g => g.Ordering).ToList();
                    if (lstChild.Count > 0)
                    {
                        foreach (var item in lstChild)
                        {
                            item.DisplayOrder = "&nbsp&nbsp" + tblCate.Ordering + "." + item.Ordering;
                            categories.Add(item);
                            var lstChild3 = lstCategories.Where(g => g.ParentID == item.ID).OrderBy(g => g.Ordering).ToList();
                            if (lstChild3.Count > 0)
                            {
                                foreach (var item3 in lstChild3)
                                {
                                    item3.DisplayOrder = "&nbsp&nbsp &nbsp&nbsp" + item.DisplayOrder + "." + item3.Ordering;
                                    categories.Add(item3);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                categories.AddRange(lstCategories);
            }
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Category/_ListData.cshtml", categories)
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            var categories = new List<Category>();
            var lstCategories = categoryRepository.GetAll().OrderBy(x => x.Ordering).ToList();
            var lstParents = lstCategories.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var tblCate in lstParents)
                {
                    tblCate.DisplayOrder = tblCate.Ordering + "";
                    categories.Add(tblCate);
                    var lstChild = lstCategories.Where(g => g.ParentID == tblCate.ID).OrderBy(g => g.Ordering).ToList();
                    if (lstChild.Count > 0)
                    {
                        foreach (var item in lstChild)
                        {
                            item.DisplayOrder = "&nbsp&nbsp" + tblCate.Ordering + "." + item.Ordering;
                            categories.Add(item);
                            var lstChild3 = lstCategories.Where(g => g.ParentID == item.ID).OrderBy(g => g.Ordering).ToList();
                            if (lstChild3.Count > 0)
                            {
                                foreach (var item3 in lstChild3)
                                {
                                    item3.DisplayOrder = "&nbsp&nbsp &nbsp&nbsp" + item.DisplayOrder + "." + item3.Ordering;
                                    categories.Add(item3);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                categories.AddRange(lstCategories);
            }
            TempData["Categories"] = categories;
            return Json(RenderViewToString("~/Areas/Admin/Views/Category/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Category model)
        {
            try
            {
                var obj = categoryRepository.GetAll().FirstOrDefault(x => x.Name.Trim() == model.Name.Trim());

                if (obj != null)
                {
                    return Json(new { IsSuccess = false, Messenger = "Tên danh mục đã tồn tại" }, JsonRequestBehavior.AllowGet);
                }

                string linkSeo = "\\" + model.LinkSeo;

                if (model.ParentID == 0)
                {
                    model.Level = 1;
                }   
                else
                {
                    var parent = categoryRepository.GetAll().FirstOrDefault(x => x.ID == model.ParentID);
                    model.Level = parent != null ? parent.Level + 1 : 0;
                }
                categoryRepository.Add(model);
                return Json(new { IsSuccess = true, Messenger = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Messenger = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var categories = new List<Category>();
            var lstCategories = categoryRepository.GetAll().OrderBy(x => x.Ordering).ToList();
            var lstParents = lstCategories.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var tblCate in lstParents)
                {
                    tblCate.DisplayOrder = tblCate.Ordering + "";
                    categories.Add(tblCate);
                    var lstChild = lstCategories.Where(g => g.ParentID == tblCate.ID).OrderBy(g => g.Ordering).ToList();
                    if (lstChild.Count > 0)
                    {
                        foreach (var item in lstChild)
                        {
                            item.DisplayOrder = "&nbsp&nbsp" + tblCate.Ordering + "." + item.Ordering;
                            categories.Add(item);
                            var lstChild3 = lstCategories.Where(g => g.ParentID == item.ID).OrderBy(g => g.Ordering).ToList();
                            if (lstChild3.Count > 0)
                            {
                                foreach (var item3 in lstChild3)
                                {
                                    item3.DisplayOrder = "&nbsp&nbsp &nbsp&nbsp" + item.DisplayOrder + "." + item3.Ordering;
                                    categories.Add(item3);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                categories.AddRange(lstCategories);
            }
            TempData["Categories"] = categories;
            var obj = categoryRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Category/_Edit.cshtml", obj), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Category model)
        {
            try
            {
                string linkSeo = "\\" + model.LinkSeo;
                if (model.ParentID == 0)
                {
                    model.Level = 1;
                } 
                else
                {
                    var parent = categoryRepository.GetAll().FirstOrDefault(x=>x.ID == model.ParentID);
                    model.Level = parent != null ? parent.Level + 1 : 0;
                }
                var obj = categoryRepository.GetAll().FirstOrDefault(x => x.Name.Trim() == model.Name.Trim() && x.ID != model.ID);
                if (obj != null)
                {
                    return Json(new { IsSuccess = false, Messenger = "Tên danh mục đã tồn tại" }, JsonRequestBehavior.AllowGet);
                }
            
                categoryRepository.Edit(model);
                return Json(new { IsSuccess = true, Messenger = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Messenger = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult UpdatePosition(string value)
        {
            var arrValue = value.Split('|');
            foreach (var item in arrValue)
            {
                var id = item.Split(':')[0];
                var pos = item.Split(':')[1];
                var obj = categoryRepository.Find(Convert.ToInt32(id));
                obj.Ordering = Convert.ToInt32(pos);
                try
                {
                    categoryRepository.Edit(obj);

                }
                catch (Exception)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = string.Format("Cập nhật vị trí thất bại")
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Cập nhật vị trí thành công",
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                var cate = categoryRepository.Find(id);
               
                if (cate != null)
                {
                    var child = categoryRepository.GetAll().Where(x => x.ParentID == cate.ID);
                    if (child.Any())
                    {
                        return Json(new { IsSuccess = false, Messenger = "Danh mục đang chứa danh mục con - Xóa danh mục con trước khi xóa Danh mục này" }, JsonRequestBehavior.AllowGet);
                    }
                     
                    else
                    {
                        var news = newsRepository.GetAll().Where(x => x.CategoryId == cate.ID);
                        if (news.Any())
                        {
                            return Json(new { IsSuccess = false, Messenger = "Danh mục đang chứa Tin tức - Xóa Tin tức trước khi xóa Danh mục này" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
               
                categoryRepository.Delete(id);
                return Json(new { IsSuccess = true, Messenger = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Messenger = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
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
                    categoryRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} danh mục", count),
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
