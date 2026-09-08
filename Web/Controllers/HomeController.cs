using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        readonly IWordRepository wordRepository = new WordRepository();
        public ActionResult Index(string langCode)
        {
            string culture = string.Empty;
            if (!string.IsNullOrEmpty(langCode))
            {
                tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x=>x.LangCode.Equals(langCode.ToUpper()));
                if(language != null)
                {
                    Session["LangCode"] = language.LangCode;
                    culture = language.FullCode;
                }
                else
                {
                    Session["LangCode"] = "EN";
                    culture = "en-US";
                }
            }
            else
            {
                tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
                if (language != null)
                {
                    langCode = language.LangCode;
                    Session["LangCode"] = language.LangCode;
                    culture = language.FullCode;
                }
                else
                {
                    langCode = "EN";
                    Session["LangCode"] = "EN";
                    culture = "en-US";
                }
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            ViewBag.GroupTrip = wordRepository.GetValueByKey("GroupTrip", langCode);  
            ViewBag.CustomizedTrips = wordRepository.GetValueByKey("CustomizedTrips", langCode);
            ViewBag.Price = Resources.Language.Price;
            ViewBag.Days = Resources.Language.Days;
            ViewBag.Nigths = Resources.Language.Nigths;
            ViewBag.Detail = Resources.Language.Detail;
            ViewBag.SeeMore = Resources.Language.SeeMore;
            ViewBag.WhyChooses = Resources.Language.WhyChooses;
            ViewBag.ChuyenGiaDuLich = Resources.Language.ChuyenGiaDuLich;
            ViewBag.BestPrice = Resources.Language.BestPrice;
            ViewBag.BestPriceDescription = Resources.Language.BestPriceDescription;
            ViewBag.YenTamDuLich = Resources.Language.YenTamDuLich;
            ViewBag.YenTamDesc = Resources.Language.YenTamDesc;
            ViewBag.StyleTour = Resources.Language.StyleTour;
            ViewBag.StyleTourDesc = Resources.Language.StyleTourDesc;
            ViewBag.SpeedSupport = Resources.Language.SpeedSupport;
            ViewBag.SpeedSupportDesc = Resources.Language.SpeedSupportDesc;

            ViewBag.TravelDestinations = wordRepository.GetValueByKey("TravelDestinations", langCode);
            ViewBag.OurSpecialists = wordRepository.GetValueByKey("OurSpecialists", langCode); 

            TempData["GroupTour"] = productTransRepository.GetByType(1, langCode, 9);

            TempData["PrivateTour"] = productTransRepository.GetByType(2, langCode, 3);

            TempData["Countries"] = countryRepository.GetCountryViewModelByLangCode(langCode);

            TempData["UserModel"] = userRepository.GetAllByLangCode(langCode);

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
            ViewBag.MessagerWhatsApp = wordRepository.GetValueByKey("MessagerWhatsApp", langCode);
            ViewBag.SupportNow = wordRepository.GetValueByKey("SupportNow", langCode);
            var users = userRepository.GetAllByLangCode(langCode).ToList();
            return PartialView(users);
        }

        public PartialViewResult Footer()
        { 
            ViewBag.Slogan = Resources.Language.Slogan; 
            return PartialView();
        }
    }
}