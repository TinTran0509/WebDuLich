using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Models;
using Web.Repository;
using Web.Repository.Entity;
using Web.Resources;

namespace Web.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        readonly IProductRepository productRepository = new ProductRepository();
        readonly IMenuRepository menuRepository = new MenuRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        readonly IMenuTransRepository menuTransRepository = new MenuTransRepository(); 
        readonly IProductTransRepository productTransRepository = new ProductTransRepository();
        readonly ILocationRepository locationRepository = new LocationRepository();
        readonly ICountryRepository countryRepository = new CountryRepository();
        readonly IHotelRepository hotelRepository = new HotelRepository();
        readonly IPackageRepository _packageRepository = new PackageRepository();
        //

        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(string code, string title, int type, int page)
        {
            List<ProductModel> productModels = productTransRepository.GetByPage(code, title, type, page, 20, out int total).ToList();
             
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Product/_ListData.cshtml", productModels),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }  
         
        [HttpGet]
        public ActionResult Add()
        {
            tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
            if (language != null)
            {
                TempData["Menus"] = menuTransRepository.GetAll().
                    Where(x => x.ParentID != 0 && x.LangCode.Equals(language.LangCode)).ToList();

                //TempData["Location"] = locationRepository.GetAllLocationTrans().Where(x => x.LangCode.Equals(language.LangCode)).ToList();

                TempData["CountryTrans"] = countryRepository.GetCountryTranByLangCode(language.LangCode).ToList();
                 
            } 

            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            List<ProductLanguageViewModel> productLanguageViewModels = new List<ProductLanguageViewModel>();
          
            foreach (var lang in tbl_Languages) 
            {
                ProductLanguageViewModel productLanguageViewModel = new ProductLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                productLanguageViewModels.Add(productLanguageViewModel);
            }
            var model = new ProductCreateViewModel
            {
                Languages = productLanguageViewModels 
            };

            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ProductCreateViewModel model, string Package_Price, string ItineraryData)
        {
            try
            { 
                if (string.IsNullOrEmpty(model.ProductCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập mã sản phẩm",
                        Session = 0
                    }, JsonRequestBehavior.AllowGet);
                } 

                model.ProductCode = model.ProductCode.ToUpper();

                Product productCk = productRepository.GetAll().FirstOrDefault(x=>x.ProductCode.Equals(model.ProductCode));

                if (productCk != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Mã sản phẩm đã tồn tại",
                        Session = 0
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.Type == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn loại hình",
                        Session = 70
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.CountryID == null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn quốc gia",
                        Session = 250
                    }, JsonRequestBehavior.AllowGet);
                }  

                JavaScriptSerializer json = new JavaScriptSerializer();

                List<LocationModel> locationModels = new List<LocationModel>();
                if (!string.IsNullOrEmpty(model.LocationID))
                {
                    locationModels = json.Deserialize<List<LocationModel>>(model.LocationID);
                    locationModels = locationModels.OrderBy(x => x.SortOrder).ToList();
                }

                if (locationModels.Count == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn điểm đến",
                        Session = 300
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.MenuID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn menu",
                        Session = 320
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ảnh",
                        Session = 320
                    }, JsonRequestBehavior.AllowGet);
                }
                 
                List<ProductTran> productTrans = new List<ProductTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                            Session = 700
                        }, JsonRequestBehavior.AllowGet);
                    }

                    ProductTran productTranAdd = new ProductTran
                    {
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Description = lang.Description,
                        Contents = lang.Contents
                    };

                    productTrans.Add(productTranAdd);
                } 

                Product product = new Product
                {
                    ProductCode = model.ProductCode,
                    Image = model.Image,
                    ImageItinerary = model.ImageItinerary,
                    MenuID = model.MenuID,
                    Type = model.Type,
                    Size = model.Size,
                    CountryID = model.CountryID != null ? string.Join(",", model.CountryID) : null,
                    LocationID = locationModels.Any() ? string.Join(",", locationModels.Select(x => x.LocationId)) : null,
                    DayNumber = model.DayNumber,
                    //Itineraries = model.Itineraries,
                    Active = true
                };

                int id = productRepository.Add(product);

                List<Itinerary> itinerariesModel = new List<Itinerary>();
                 
                if (!string.IsNullOrEmpty(ItineraryData))
                {
                    itinerariesModel = json.Deserialize<List<Itinerary>>(ItineraryData);
                }

                if (itinerariesModel.Count > 0)
                {
                    productRepository.InsertItinerary(id, itinerariesModel);
                }

                foreach (var item in productTrans)
                {
                    item.ProductID = id;
                    productTransRepository.Add(item);
                }

                List<ImportantNote> importantNotes = new List<ImportantNote>();
                if (!string.IsNullOrEmpty(model.ImportantNotes))
                {
                    importantNotes = json.Deserialize<List<ImportantNote>>(model.ImportantNotes);
                } 

                if(importantNotes.Count > 0)
                {
                    productRepository.InsertImportantNote(id, importantNotes);
                } 

                List<Package_Price> package_Prices = new List<Package_Price>(); 

                if (!string.IsNullOrEmpty(Package_Price))
                {
                    package_Prices = json.Deserialize<List<Package_Price>>(Package_Price);
                }

                foreach (Package_Price package in package_Prices)
                {
                    if (package.Price == null)
                        package.Price = 0;
                    package.ProductID = id;
                    _packageRepository.AddPackagePrice(package);
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
        public ActionResult Edit(int id)
        {
            Product product = productRepository.Find(id);

            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            tbl_Languages tbl_Language = tbl_Languages.FirstOrDefault(x => x.IsDefault);

            TempData["Menus"] = menuTransRepository.GetAll().Where(x => x.ParentID != 0 && x.LangCode.Equals(tbl_Language.LangCode)).ToList();

            TempData["Location"] = locationRepository.GetAllLocationTrans().Where(x => x.LangCode.Equals(tbl_Language.LangCode)).ToList();
             
            TempData["CountryTrans"] = countryRepository.GetCountryTranByLangCode(tbl_Language.LangCode).ToList();
             
            List<ProductTran> lstProductTrans = productTransRepository.GetAll().Where(x => x.ProductID == id).ToList();
              
            List<ProductLanguageViewModel> productLanguageViewModels = new List<ProductLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                ProductTran productTran_Edit = lstProductTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (productTran_Edit != null)
                {
                    ProductLanguageViewModel productLanguageViewModel = new ProductLanguageViewModel
                    {
                        ID = productTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = productTran_Edit.Title,
                        Description = productTran_Edit.Description,
                        Contents = productTran_Edit.Contents,
                    };
                    productLanguageViewModels.Add(productLanguageViewModel);
                }
            }

            JavaScriptSerializer json = new JavaScriptSerializer();

            string sImportantNote = string.Empty;
            List<ImportantNote> importantNotes = productRepository.GetAllImportantNote().Where(x=>x.ProductID == id).ToList();
            if(importantNotes.Any())
            { 
                sImportantNote = json.Serialize(importantNotes);
            }

            List<int> listDay = new List<int>();
            List<Itinerary> itineraries = productTransRepository.GetItineraryByProductID(id).ToList();
            List<ItineraryModel> itineraryModels = new List<ItineraryModel>();
            if(itineraries != null)
            {
                foreach (var item in itineraries)
                {
                    int day = item.Day != null ? (int)item.Day : 0;
                    ItineraryModel itineraryModel = new ItineraryModel();
                    itineraryModel.day = day;

                    if (!listDay.Contains(day))
                    {
                        List<Itinerary> titls = itineraries.Where(x => x.Day == day).ToList();

                        if (titls != null)
                        {
                            itineraryModel.title = new Title
                            {
                                en = titls.FirstOrDefault(x => x.LangCode.ToUpper() == "EN") != null ? titls.FirstOrDefault(x => x.LangCode.ToUpper() == "EN").Title : "",
                                es = titls.FirstOrDefault(x => x.LangCode.ToUpper() == "ES") != null ? titls.FirstOrDefault(x => x.LangCode.ToUpper() == "ES").Title : ""
                            };

                            itineraryModel.content = new Models.Content
                            {
                                en = titls.FirstOrDefault(x => x.LangCode.ToUpper() == "EN") != null ? titls.FirstOrDefault(x => x.LangCode.ToUpper() == "EN").Contents : "",
                                es = titls.FirstOrDefault(x => x.LangCode.ToUpper() == "ES") != null ? titls.FirstOrDefault(x => x.LangCode.ToUpper() == "ES").Contents : ""
                            };
                        }
                        itineraryModels.Add(itineraryModel);
                    } 

                    listDay.Add(day);
                }
            }
             
            string sItineraries = "";
            if (itineraryModels.Any())
            {
                sItineraries = json.Serialize(itineraryModels);
            }

            var model = new ProductCreateViewModel
            {
                ID = id,
                ProductCode = product.ProductCode,
                Type = product.Type,
                MenuID = product.MenuID,
                Active = product.Active,
                Image = product.Image,
                ImageItinerary = product.ImageItinerary,
                Size = product.Size,
                DayNumber = product.DayNumber != null ? (int)product.DayNumber : 0, 
                CreatedDate = product.CreatedDate,
                Itineraries = sItineraries,
                Languages = productLanguageViewModels,
                ImportantNotes = sImportantNote
            };

            if (!string.IsNullOrEmpty(product.CountryID))
            {
                List<CountryTran> countryTrans = countryRepository.GetByCountryID(product.CountryID, tbl_Language.LangCode).ToList();
                if (countryTrans != null)
                {
                    List<int> countryIDs = new List<int>();
                    string sCountryTransIDs = string.Empty;
                    foreach (var item in countryTrans)
                    {
                        countryIDs.Add(item.CountryID);
                        sCountryTransIDs += !string.IsNullOrEmpty(sCountryTransIDs) ? ";" + item.ID : item.ID + "";
                    }
                    ViewBag.CountryIDs = countryIDs;
                    ViewBag.SelectedCountries = sCountryTransIDs;
                }
            }

            List<LocationViewModel> locationTrans = new List<LocationViewModel>();
            if (!string.IsNullOrEmpty(product.LocationID))
            {
                List<string> locationIDs = product.LocationID.Split(',').ToList();
                List<LocationViewModel> _locationTrans = locationRepository.GetByLocationIDs(product.LocationID, tbl_Language.LangCode).ToList();
                foreach (var item in locationIDs)
                {
                    LocationViewModel locationViewModel = _locationTrans.Where(x => x.LocationID == Convert.ToInt32(item)).FirstOrDefault();
                    locationTrans.Add(locationViewModel);
                }
            }
            TempData["LocationTrans"] = locationTrans;

            if (!string.IsNullOrEmpty(product.HotelID))
            {
                List<HotelTran> hotelTrans = hotelRepository.GetByHotelID(product.HotelID, tbl_Language.LangCode).ToList();
                if (hotelTrans != null)
                {
                    List<int> hotelIDs = new List<int>();
                    string sHotelTransIDs = string.Empty;
                    foreach (var item in hotelTrans)
                    {
                        hotelIDs.Add(item.HotelID);
                        sHotelTransIDs += !string.IsNullOrEmpty(sHotelTransIDs) ? ";" + item.ID : item.ID + "";
                    }
                    ViewBag.HotelIDs = hotelIDs;
                    ViewBag.SelectedHotels = sHotelTransIDs;
                }
            }

            List<Package_Price> package_Prices = _packageRepository.GetAllPackagePrice().Where(x=>x.ProductID == id).ToList();
            TempData["Package3"] = package_Prices.Where(x => x.PackageID == 3).ToList();
            TempData["Package4"] = package_Prices.Where(x => x.PackageID == 4).ToList();
            TempData["Package5"] = package_Prices.Where(x => x.PackageID == 5).ToList();
            TempData["Package6"] = package_Prices.Where(x => x.PackageID == 6).ToList();

            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductCreateViewModel model, string Package_Price, string ItineraryData)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ProductCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập mã sản phẩm",
                        Session = 0
                    }, JsonRequestBehavior.AllowGet);
                }

                model.ProductCode = model.ProductCode.ToUpper();

                Product productCk = productRepository.GetAll().FirstOrDefault(x => x.ProductCode.Equals(model.ProductCode) && x.ID != model.ID);

                if (productCk != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Mã sản phẩm đã tồn tại",
                        Session = 0
                    }, JsonRequestBehavior.AllowGet);
                } 

                List<ProductTran> productTrans = new List<ProductTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                            Session = 700
                        }, JsonRequestBehavior.AllowGet);
                    }

                    ProductTran productTranEdit = new ProductTran
                    {
                        ID = lang.ID, 
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        Description = lang.Description,
                        Contents = lang.Contents
                    };
                    productTrans.Add(productTranEdit);
                }

                JavaScriptSerializer json = new JavaScriptSerializer();

                List<LocationModel> locationModels = new List<LocationModel>();
                if (!string.IsNullOrEmpty(model.LocationID))
                { 
                    locationModels = json.Deserialize<List<LocationModel>>(model.LocationID);
                    locationModels = locationModels.OrderBy(x=>x.SortOrder).ToList();
                }

                Product product = new Product
                {
                    ID = model.ID,
                    ProductCode = model.ProductCode,
                    Image = model.Image,
                    ImageItinerary = model.ImageItinerary,
                    MenuID = model.MenuID,
                    Active = true,
                    Type = model.Type,
                    Size = model.Size,
                    DayNumber = model.DayNumber,
                   // Itineraries = model.Itineraries,
                    CountryID =  model.CountryID != null ? string.Join(",", model.CountryID) : null,
                    LocationID = locationModels.Any() ? string.Join(",", locationModels.Select(x=>x.LocationId)) : null
                };

                productRepository.Edit(product, productTrans);

                List<Itinerary> itinerariesModel = new List<Itinerary>(); 

                if (!string.IsNullOrEmpty(ItineraryData))
                {
                    itinerariesModel = json.Deserialize<List<Itinerary>>(ItineraryData);
                }

                if (itinerariesModel.Count > 0)
                {
                    productRepository.InsertItinerary(product.ID, itinerariesModel);
                }

                List<ImportantNote> importantNotes = new List<ImportantNote>();
                if (!string.IsNullOrEmpty(model.ImportantNotes))
                {
                    importantNotes = json.Deserialize<List<ImportantNote>>(model.ImportantNotes);
                }

                if (importantNotes.Count > 0)
                {
                    productRepository.InsertImportantNote(model.ID, importantNotes);
                }

                List<Package_Price> package_Prices = new List<Package_Price>(); 

                if (!string.IsNullOrEmpty(Package_Price))
                {
                    package_Prices = json.Deserialize<List<Package_Price>>(Package_Price);
                }

                foreach (Package_Price package in package_Prices)
                {
                    if (package.Price == null)
                        package.Price = 0;
                    if(package.ID == 0)
                    {
                        package.ProductID = product.ID;
                        _packageRepository.AddPackagePrice(package);
                    }
                    else
                    {
                        _packageRepository.UpdatePackagePrice(package);
                    } 
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                List<ProductTran> productTranList = productTransRepository.GetAll().Where(m => m.ProductID == id).ToList();
                productRepository.Delete(id, productTranList);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa thành công",
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationByCountry(string  ids)
        {
            try
            {
                var locationTrans = locationRepository.GetLocationTranByCoutryID("EN", ids).ToList(); 

                return Json(new
                {
                    Data = locationTrans
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            { 
                return Json(new
                {
                    Messager ="Lấy dữ liệu thất bại"
                }, JsonRequestBehavior.AllowGet); ;
            } 
        }

        [HttpGet]
        public ActionResult AddItinerary(int id)
        {
            Product product = productRepository.Find(id);

            return Json(RenderViewToString("~/Areas/Admin/Views/Product/_Itinerary.cshtml", product), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddItinerary(int ID, string Itineraries, string ImageItinerary, string ItineraryData)
        { 
            try
            {
                Product product = new Product();
                product.ID = ID;
                product.ImageItinerary = ImageItinerary;
                product.Itineraries = Itineraries;
                productRepository.Update(product);

                List<Itinerary> itinerariesModel = new List<Itinerary>();
                JavaScriptSerializer json = new JavaScriptSerializer();

                if (!string.IsNullOrEmpty(ItineraryData))
                {
                    itinerariesModel = json.Deserialize<List<Itinerary>>(ItineraryData);
                }

                if (itinerariesModel.Count > 0) 
                {
                    productRepository.InsertItinerary(product.ID, itinerariesModel);
                }

                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Lưu thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Lấy dữ liệu thất bại"
                }, JsonRequestBehavior.AllowGet);  
            }
        }

        public ActionResult ProductDetail(int id)
        {
            Product product = productRepository.Find(id);

            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            tbl_Languages tbl_Language = tbl_Languages.FirstOrDefault(x => x.IsDefault);
  
            List<ProductTran> lstProductTrans = productTransRepository.GetAll().Where(x => x.ProductID == id).ToList();

            List<ProductLanguageViewModel> productLanguageViewModels = new List<ProductLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                ProductTran productTran_Edit = lstProductTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (productTran_Edit != null)
                {
                    ProductLanguageViewModel productLanguageViewModel = new ProductLanguageViewModel
                    {
                        ID = productTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = productTran_Edit.Title,
                        Description = productTran_Edit.Description,
                        Contents = productTran_Edit.Contents,
                    };
                    productLanguageViewModels.Add(productLanguageViewModel);
                }
            }
            string sImportantNote = string.Empty;
            List<ImportantNote> importantNotes = productRepository.GetAllImportantNote().Where(x => x.ProductID == id).ToList();
            if (importantNotes.Any())
            {
                JavaScriptSerializer json = new JavaScriptSerializer();
                sImportantNote = json.Serialize(importantNotes);
            }
            var model = new ProductCreateViewModel
            {
                ID = id,
                ProductCode = product.ProductCode,
                Type = product.Type,
                MenuID = product.MenuID,
                Active = product.Active,
                Image = product.Image,
                ImageItinerary = product.ImageItinerary,
                Size = product.Size,
                DayNumber = product.DayNumber != null ? (int)product.DayNumber : 0,
                CreatedDate = product.CreatedDate,
                Itineraries = product.Itineraries,
                Languages = productLanguageViewModels,
                ImportantNotes = sImportantNote
            };
            if (!string.IsNullOrEmpty(product.CountryID))
            {
                List<CountryTran> countryTrans = countryRepository.GetByCountryID(product.CountryID, tbl_Language.LangCode).ToList();
                if (countryTrans != null)
                {
                    List<int> countryIDs = new List<int>();
                    string sCountryTransIDs = string.Empty;
                    foreach (var item in countryTrans)
                    {
                        countryIDs.Add(item.CountryID);
                        sCountryTransIDs += !string.IsNullOrEmpty(sCountryTransIDs) ? ";" + item.ID : item.ID + "";
                    } 
                    ViewBag.CountryIDs = countryIDs;
                    ViewBag.SelectedCountries = sCountryTransIDs;
                }
            }
            if (!string.IsNullOrEmpty(product.LocationID))
            {
                string locationEn = string.Empty;
                string locationEs = string.Empty;
                List<LocationViewModel> locationTrans = new List<LocationViewModel>();
                if (!string.IsNullOrEmpty(product.LocationID))
                {
                    List<string> locationIDs = product.LocationID.Split(',').ToList();
                    List<LocationViewModel> _locationTrans = locationRepository.GetByLocationIDs(product.LocationID, tbl_Language.LangCode).ToList();
                    foreach (var item in locationIDs)
                    {
                        LocationViewModel locationViewModel = _locationTrans.Where(x => x.LocationID == Convert.ToInt32(item)).FirstOrDefault();
                        locationTrans.Add(locationViewModel);
                    }
                }
                TempData["LocationTrans"] = locationTrans;  
            }
             
            List<Package_Price> package_Prices = _packageRepository.GetAllPackagePrice().Where(x => x.ProductID == id).ToList();
            TempData["Package3"] = package_Prices.Where(x => x.PackageID == 3).ToList();
            TempData["Package4"] = package_Prices.Where(x => x.PackageID == 4).ToList();
            TempData["Package5"] = package_Prices.Where(x => x.PackageID == 5).ToList();
            TempData["Package6"] = package_Prices.Where(x => x.PackageID == 6).ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/Product/_ProductDetail.cshtml", model), JsonRequestBehavior.AllowGet);
        }
    }
}
