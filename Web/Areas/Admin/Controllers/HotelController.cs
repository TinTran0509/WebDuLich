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
    public class HotelController : BaseController
    {
        readonly IHotelRepository hotelRepository = new HotelRepository();
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
            var model = hotelRepository.GetAll().OrderBy(x=>x.ID).ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Hotel/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<HotelLanguageViewModel> HotelLanguageViewModels = new List<HotelLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                HotelLanguageViewModel HotelLanguageViewModel = new HotelLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                HotelLanguageViewModels.Add(HotelLanguageViewModel);
            }
            var model = new HotelCreateViewModel
            {
                Languages = HotelLanguageViewModels
            };
            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(HotelCreateViewModel model)
        {
            try
            { 
                List<HotelTran> HotelTrans = new List<HotelTran>();

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

                    HotelTran HotelTranAdd = new HotelTran
                    {
                        Name = lang.Name, 
                        Description = lang.Description,
                        LangCode = lang.LangCode
                    };

                    HotelTrans.Add(HotelTranAdd);
                }

                Hotel Hotel = new Hotel
                {
                    Image = model.Image,
                    Active = model.Active,
                    Rating = model.Rating
                }; 
                
                int id = hotelRepository.Create(Hotel);

                foreach (var item in HotelTrans)
                {
                    item.HotelID = id;
                    hotelRepository.Add(item);
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

            List<HotelTran> lstHotelTrans = hotelRepository.GetAllHotelTrans().Where(x => x.HotelID == id).ToList();

            Hotel hotel = hotelRepository.Find(id);

            List<HotelLanguageViewModel> hotelLanguageViewModels = new List<HotelLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                HotelTran hotelTran_Edit = lstHotelTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (hotelTran_Edit != null)
                {
                    HotelLanguageViewModel hotelLanguageViewModel = new HotelLanguageViewModel
                    {
                        ID = hotelTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Name = hotelTran_Edit.Name,
                        Description = hotelTran_Edit.Description
                    };
                    hotelLanguageViewModels.Add(hotelLanguageViewModel);
                }
            }
            var model = new HotelCreateViewModel
            {
                ID = id,
                Image = hotel.Image,
                Active = hotel.Active,
                Rating = hotel.Rating,
                Languages = hotelLanguageViewModels
            };
            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(HotelCreateViewModel model)
        {
            try
            {
                List<HotelTran> hotelTrans = new List<HotelTran>();
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

                    HotelTran hotelTranEdit = new HotelTran
                    {
                        ID = lang.ID,
                        Name = lang.Name,
                        Description = lang.Description
                    };
                    hotelTrans.Add(hotelTranEdit);
                } 
                
                Hotel hotel = new Hotel();
                hotel.ID = model.ID;
                hotel.Image = model.Image;
                hotel.Active = model.Active;
                hotel.Rating = model.Rating;

                hotelRepository.Edit(hotel);

                foreach (var item in hotelTrans)
                {
                    hotelRepository.Update(item);
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
                var obj = hotelRepository.Find(id);
                hotelRepository.Delete(id);
               
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
