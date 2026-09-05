using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Model;
using Web.Model.CustomModel;
using Web.Model.Domain;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        readonly ICountryRepository countryRepository = new CountryRepository();
        readonly ISliderRepository slideRepository = new SliderRepository();
        readonly INewsRepository newsRepository = new NewsRepository();
        readonly ICategoryRepository categoryRepository = new CategoryRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        readonly IMenuHomeRepository menuRepository = new MenuHomeRepository();
        readonly IUserRepository userRepository = new UserRepository();
        readonly IProductTransRepository productTransRepository = new ProductTransRepository();
        readonly IIntroductionTransRepository introductionTransRepository = new IntroductionTransRepository();
        public ActionResult Index(string langCode)
        {
            if (!string.IsNullOrEmpty(langCode))
            {
                tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x=>x.LangCode.Equals(langCode.ToUpper()));
                if(language != null)
                {
                    Session["LangCode"] = language.LangCode;
                }
                else
                {
                    Session["LangCode"] = "EN";
                }
            }
            else
            {
                tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
                if (language != null)
                {
                    langCode = language.LangCode;
                    Session["LangCode"] = language.LangCode;
                }
                else
                {
                    langCode = "EN";
                    Session["LangCode"] = "EN";
                }
            }

            TempData["GroupTour"] = productTransRepository.GetByType(1, langCode, 9);

            TempData["PrivateTour"] = productTransRepository.GetByType(2, langCode, 3);

            return View();
        } 

        public PartialViewResult Menu()
        { 
            string langCode = (string)Session["LangCode"];
            if(langCode == null)
            {
                langCode = "EN";
            } 
            var menus = menuRepository.GetByLangCode(langCode).ToList();
            return PartialView(menus);
        } 

        public PartialViewResult Slider()
        {
            var slider = slideRepository.GetAll().ToList();
            return PartialView(slider);
        }

        public PartialViewResult Header()
        {
            List<tbl_Languages> languages = languageRepository.GetByActive().ToList();
            string langCode = (string)Session["LangCode"];
            if (langCode != null)
            {
                ViewBag.UserModel = userRepository.GetByLangCode(langCode);
            }
            return PartialView(languages);
        }

        public PartialViewResult Banner()
        {
            string langCode = (string)Session["LangCode"];
            IntroductionViewModel introduction = introductionTransRepository.GetByLangCode(langCode);
            return PartialView(introduction);
        }

        public PartialViewResult Language()
        {
            var language = languageRepository.GetAll();
            return PartialView(language);
        }

        public PartialViewResult Support()
        {
            string langCode = (string)Session["LangCode"];
            var users = userRepository.GetAllByLangCode(langCode).ToList();
            return PartialView(users);
        }
    }
}