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
            
            return View();
        }  
    }
}