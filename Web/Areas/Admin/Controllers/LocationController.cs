using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class LocationController : BaseController
    {
        readonly ILocationRepository locationRepository = new LocationRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();

        [Authorize(Roles = "Index")]
        public ActionResult Index()
        { 
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
            var model = locationRepository.GetAll().OrderBy(x=>x.ID).ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Location/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<LocationLanguageViewModel> locationLanguageViewModels = new List<LocationLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                LocationLanguageViewModel locationLanguageViewModel = new LocationLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                locationLanguageViewModels.Add(locationLanguageViewModel);
            }
            var model = new LocationCreateViewModel
            {
                Languages = locationLanguageViewModels
            };
            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(LocationCreateViewModel model)
        {
            try
            { 
                List<LocationTran> locationTrans = new List<LocationTran>();

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

                    LocationTran locationTranAdd = new LocationTran
                    {
                        Name = lang.Name, 
                        Description = lang.Description,
                        LangCode = lang.LangCode
                    };

                    locationTrans.Add(locationTranAdd);
                }

                Location location = new Location
                {
                    Image = model.Image,
                    Active = model.Active
                }; 
                
                int id = locationRepository.Create(location);

                foreach (var item in locationTrans)
                {
                    item.LocationID = id;
                    locationRepository.Add(item);
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
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<LocationTran> lstLocationTrans = locationRepository.GetAllLocationTrans().Where(x => x.LocationID == id).ToList();

            Location location = locationRepository.Find(id);

            List<LocationLanguageViewModel> locationLanguageViewModels = new List<LocationLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                LocationTran LocationTran_Edit = lstLocationTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (LocationTran_Edit != null)
                {
                    LocationLanguageViewModel locationLanguageViewModel = new LocationLanguageViewModel
                    {
                        ID = LocationTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Name = LocationTran_Edit.Name,
                        Description = LocationTran_Edit.Description
                    };
                    locationLanguageViewModels.Add(locationLanguageViewModel);
                }
            }
            var model = new LocationCreateViewModel
            {
                ID = id,
                Image = location.Image,
                Active = location.Active,
                Languages = locationLanguageViewModels
            };
            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LocationCreateViewModel model)
        {
            try
            {
                List<LocationTran> locationTrans = new List<LocationTran>();
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

                    LocationTran locationTranEdit = new LocationTran
                    {
                        ID = lang.ID,
                        Name = lang.Name,
                        Description = lang.Description
                    };
                    locationTrans.Add(locationTranEdit);
                } 
                
                Location location = new Location();
                location.ID = model.ID;
                location.Image = model.Image;
                location.Active = model.Active;

                locationRepository.Edit(location);

                foreach (var item in locationTrans)
                {
                    locationRepository.Update(item);
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
                var obj = locationRepository.Find(id);
                locationRepository.Delete(id);
               
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
    }
}
