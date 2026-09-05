using CMS.IRepository;
using CMS.Reporitory;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Model;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Controllers
{
    public class CategoryController : BaseController
    {
        ICategoryRepository categoryRepository = new CategoryRepository();
        INewsRepository newsRepository = new NewsRepository(); 
        // GET: Category

        public ActionResult Index(string linkseo)
        { 
            var category = categoryRepository.GetAll().Where(x => x.LinkSeo.Equals(linkseo)).FirstOrDefault();
            string title = category != null ? category.Name : "";
            ViewBag.CateType = category != null ? category.Type : 0;
            int categoryId = category != null ? category.ID : 0;
            ViewBag.ID = categoryId;
            ViewBag.Link = linkseo;
            ViewBag.Title = title;
            ViewBag.UrlImage = ""; 
            List<News> model = category != null ? category.News.ToList() : new List<News>();
            List<News> newsRelated = new List<News>();
            List<News> lstNews = new List<News>();
            News news = new News();
            if (category != null && category.News.Count == 1)
            {
                news = category.News.FirstOrDefault();
                newsRelated = category.News.Where(n => n.CategoryId == category.ID && n.ID != news.ID).ToList();
                lstNews = newsRepository.GetAll().Where(x => x.ID != news.ID && (x.Type == 1 || x.Type == 2)).OrderByDescending(x => x.CreatedDate).ToList();
            } 
            ViewBag.Relateds = newsRelated;
            ViewBag.LstNews = lstNews;
            return View(model);
        }

        public ActionResult Search(string search,int category_id)
        {
            var category = categoryRepository.GetAll().Where(x => x.ID.Equals(category_id)).FirstOrDefault();
           
            List<News> lstNews = new List<News>(); 
            string title = "", description = "", urlImage = "", linkseo="";
            Session["CategoryId"] = category_id;
            if (category != null)
            {
                title = category.Name;
                description = WebConfigurationManager.AppSettings["CategoryDescription"];
                urlImage = WebConfigurationManager.AppSettings["UrlImage"];
                if (category.Type == (int)CategoryType.BaiViet)
                {
                    lstNews = newsRepository.GetAll().Where(x => x.CategoryId == category.ID).ToList();
                } 
            }

            ViewBag.CateType = category != null ? category.Type : 0;
            ViewBag.Link = linkseo;
            ViewBag.Title = title;
            ViewBag.Description = description;
            ViewBag.UrlImage = urlImage;
            ViewBag.ListNews = lstNews; 
            return View();
        }

        public ActionResult LoadCategory()
        {
            var categories = categoryRepository.GetAll().ToList();
            return PartialView(categories);
        }

        public ActionResult ListCate(int cateType, int cateId, int pageIndex, int pageSize)
        {
            List<News> lstNews = new List<News>(); 
            
            var totalAdv = 0;
            var model = newsRepository.GetAll().Where(x => x.CategoryId == cateId);
            totalAdv = model.Count();
            lstNews = model.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Views/Category/ListNews.cshtml", lstNews),
                totalPages = Math.Ceiling(((double)totalAdv / pageSize)),
            }, JsonRequestBehavior.AllowGet);
        } 
    }
}