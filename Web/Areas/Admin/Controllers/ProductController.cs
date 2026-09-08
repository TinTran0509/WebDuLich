using Excel.Log.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        //

        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page)
        {
            List<ProductModel> productModels = new List<ProductModel>();    
            var products = productRepository.GetAll().ToList();
            foreach (var item in products)
            { 
                List<ProductModel> productTrans = productTransRepository.GetByProductID(item.ID).ToList();
                productModels.AddRange(productTrans);
            }
            var total = 0;
            //model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
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

                TempData["Hotels"] = hotelRepository.GetAllHotelTrans().Where(x => x.LangCode.Equals(language.LangCode)).ToList();
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
        public ActionResult Add(ProductCreateViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ProductCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập mã sản phẩm",
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
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.Type == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn loại hình",
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.CountryID.Count == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn quốc gia",
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.MenuID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn chủ đề",
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!model.LocationID.Any())
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn điểm đến",
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ảnh",
                    }, JsonRequestBehavior.AllowGet);
                }

                List<ProductTran> productTrans = new List<ProductTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Contents))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm nội dung " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    } 

                    ProductTran bannerTranAdd = new ProductTran
                    {
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Description = lang.Description,
                        Contents = lang.Contents
                    };

                    productTrans.Add(bannerTranAdd);
                }

                Product product = new Product
                {
                    ProductCode = model.ProductCode,
                    Image = model.Image,
                    MenuID = model.MenuID,
                    Type = model.Type,
                    Price = model.Price,
                    Size = model.Size,
                    CountryID = string.Join(",", model.CountryID), 
                    LocationID = string.Join(",", model.LocationID),
                    HotelID = string.Join(",", model.HotelID),
                    DayNumber = model.DayNumber,
                    Active = true
                };

                int id = productRepository.Add(product); 

                foreach (var item in productTrans)
                {
                    item.ProductID = id;
                    productTransRepository.Add(item);
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

            //int countryId = product.CountryID != null ? (int)product.CountryID : 0;
            int countryId = 0;
            if (countryId == 0)
            {
                TempData["Location"] = locationRepository.GetAllLocationTrans().Where(x=>x.LangCode.Equals(tbl_Language.LangCode)).ToList();
            }
            else
            {
                //TempData["Location"] = locationRepository.GetLocationTranByCoutryID(tbl_Language.LangCode, countryId).ToList();
            } 

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

            var model = new ProductCreateViewModel
            {
                ID = id,
                ProductCode = product.ProductCode,
                Type = product.Type,
                MenuID = product.MenuID,
                Active = product.Active,
                Image = product.Image,
                Price = product.Price != null ? (double)product.Price : 0,
                Size = product.Size != null ? (int)product.Size : 0,
                DayNumber = product.DayNumber != null ? (int)product.DayNumber : 0,
                //CountryID = product.CountryID != null ? (int)product.CountryID : 0,
                CreatedDate = product.CreatedDate,
                Languages = productLanguageViewModels
            };
            //if (!string.IsNullOrEmpty(product.LocationID))
            //{
            //    List<LocationTran> locationTrans = locationRepository.GetByLocationID(product.LocationID, tbl_Language.LangCode).ToList();
            //    if(locationTrans != null)
            //    {
            //        string locationTransIDs = string.Empty;
            //        foreach (var item in locationTrans)
            //        {
            //            locationTransIDs += !string.IsNullOrEmpty(locationTransIDs) ? ";" + item.ID : item.ID + "";
            //        }
            //        ViewBag.SelectedLocation = locationTransIDs;
            //    } 
            //} 
            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductCreateViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ProductCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập mã sản phẩm",
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
                    }, JsonRequestBehavior.AllowGet);
                }

                List<ProductTran> lstProductTrans = productTransRepository.GetAll().Where(x => x.ProductID == model.ID).ToList();

                List<ProductTran> productTrans = new List<ProductTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Contents))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm nội dung " + lang.LangName,
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

                Product product = new Product
                {
                    ID = model.ID,
                    ProductCode = model.ProductCode,
                    Image = model.Image,
                    MenuID = model.MenuID,
                    Active = true,
                    Type = model.Type,
                    Price = model.Price,
                    Size = model.Size,
                    DayNumber = model.DayNumber,
                    //CountryID = model.CountryID,
                    //LocationID = string.Join(",", model.LocationID)
                };

                productRepository.Edit(product, productTrans);

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
    }
}
