
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Model;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Controllers
{
    public class DichVuController : BaseController
    {
        readonly  INewsRepository newsRepository = new NewsRepository();
        // GET: News
        public ActionResult Index()
        {
            var model = newsRepository.GetAll().Where(x => x.Type == 2);
            return View(model);
        }
         
        public ActionResult LoadData(int pageIndex, int pageSize)
        {
            var model = newsRepository.GetAll().Where(x => x.Type == (int)NewsType.BaiViet && x.CategoryId == 0);
           
            var totalAdv = model.Count();
            model = model.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Views/Blog/ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)totalAdv / pageSize)),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(string linkseo)
        {
            var news = newsRepository.GetAll().FirstOrDefault(x => x.LinkSeo.Equals(linkseo) && x.Type == 2);
            string title = "", description = "", urlImage = "";
            List<News> relateds = new List<News>();
            List<News> lstNews = new List<News>();
            if (news != null)
            {
                relateds = newsRepository.GetAll().Where(x => x.CategoryId == news.CategoryId && x.ID != news.ID).ToList();
                title = news.MetaTitle;
                lstNews = newsRepository.GetAll().Where(x => x.ID != news.ID && x.Type == 2).OrderByDescending(x => x.CreatedDate).ToList();
            }

            string path = "";
            ViewBag.Related = relateds;
            ViewBag.LstNews = lstNews;
            ViewBag.Link = linkseo;
            ViewBag.MetaTitle = title;
            ViewBag.Description = description;
            ViewBag.UrlImage = urlImage;
            ViewBag.BreadCrumb = path;

            return View(news);
        }
    }
}