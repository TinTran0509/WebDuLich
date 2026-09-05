
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
    public class ContactController : BaseController
    {
        ICategoryRepository categoryRepository = new CategoryRepository();
        readonly  INewsRepository newsRepository = new NewsRepository();
        // GET: Contact
        public ActionResult Index()
        {
            var category = categoryRepository.GetAll().Where(x => x.LinkSeo.Equals("lien-he")).FirstOrDefault();
            var model = category.News;
            return View(model);
        }  
    }
}