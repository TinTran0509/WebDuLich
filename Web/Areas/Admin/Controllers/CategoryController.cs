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
using Web.Model.CustomModel;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository categoryRepository = new CategoryRepository();
        private readonly ILanguageRepository languageRepository = new LanguageRepository();
        private readonly INewsRepository newsRepository = new NewsRepository();

        // GET: Category
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int page)
        {
            List<CategoryModel> categoryModels = new List<CategoryModel>();
            var categories = categoryRepository.GetAll().OrderBy(x=>x.Ordering).ToList();
            foreach (var item in categories)
            {
                List<CategoryModel> bannerTrans = categoryRepository.GetCategoryTranByCategoryID(item.ID).ToList();
                categoryModels.AddRange(bannerTrans);
            }
            var total = 0; 
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Category/_ListData.cshtml", categoryModels),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Create()
        {
            tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
            
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            List<CategoryLanguageViewModel> categoriesLanguageViewModels = new List<CategoryLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                CategoryLanguageViewModel categoryLanguageViewModel = new CategoryLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                categoriesLanguageViewModels.Add(categoryLanguageViewModel);
            }
            var model = new CategoryCreateViewModel
            {
                Languages = categoriesLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CategoryCreateViewModel model)
        {
            try
            {   
                List<CategoryTran> categoriesTrans = new List<CategoryTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Name))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tên " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    CategoryTran categoryTranAdd = new CategoryTran
                    {
                        Name = lang.Name,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Name),
                        LangCode = lang.LangCode 
                    };

                    categoriesTrans.Add(categoryTranAdd);
                }

                Category category = new Category
                {
                    Ordering = model.Ordering 
                };

                int id = categoryRepository.Add(category);

                foreach (var item in categoriesTrans)
                {
                    item.CategoryID = id;
                    categoryRepository.AddTrans(item);
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
        [HttpGet]
        public ActionResult Edit(int id)
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            tbl_Languages tbl_Language = tbl_Languages.FirstOrDefault(x => x.IsDefault);
            
            List<CategoryTran> categoriesTrans = categoryRepository.GetAllTrans().Where(x => x.CategoryID == id).ToList();

            Category category = categoryRepository.Find(id);

            List<CategoryLanguageViewModel> categoriesLanguageViewModels = new List<CategoryLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                CategoryTran categoryTran_Edit = categoriesTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (categoryTran_Edit != null)
                {
                    CategoryLanguageViewModel categoryLanguageViewModel = new CategoryLanguageViewModel
                    {
                        ID = categoryTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Name = categoryTran_Edit.Name 
                    };
                    categoriesLanguageViewModels.Add(categoryLanguageViewModel);
                }
            }
            var model = new CategoryCreateViewModel
            {
                ID = id,
                Ordering = (int)category.Ordering, 
                Languages = categoriesLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CategoryCreateViewModel model)
        {
            try
            { 
                List<CategoryTran> categoriesTrans = new List<CategoryTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Name))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tên " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    CategoryTran categoryTranEdit = new CategoryTran
                    {
                        ID = lang.ID,
                        Name = lang.Name,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Name) 
                    };
                    categoriesTrans.Add(categoryTranEdit);
                }

                Category category = new Category();
                category.ID = model.ID;
                category.Ordering = model.Ordering; 

                categoryRepository.Edit(category);

                foreach (var item in categoriesTrans)
                {
                    categoryRepository.EditTrans(item);
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
        public ActionResult Delete(int id)
        {
            try
            {
                var cate = categoryRepository.Find(id);
               
                if (cate != null)
                {
                    var news = newsRepository.GetAll().Where(x => x.CategoryId == cate.ID);
                    if (news.Any())
                    {
                        return Json(new { IsSuccess = false, Messenger = "Danh mục đang được tham chiếu tới bảng Tin tức" }, JsonRequestBehavior.AllowGet);
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
    }
}
