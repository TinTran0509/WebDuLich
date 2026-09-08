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
    public class CountryController : BaseController
    {
        readonly ICountryRepository countryRepository = new CountryRepository();
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
            var model = countryRepository.GetAll().OrderBy(x=>x.ID).ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Country/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<CountryLanguageViewModel> countryLanguageViewModels = new List<CountryLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                CountryLanguageViewModel countryLanguageViewModel = new CountryLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                countryLanguageViewModels.Add(countryLanguageViewModel);
            }
            var model = new CountryCreateViewModel
            {
                Languages = countryLanguageViewModels
            };
            return Json(RenderViewToString("~/Areas/Admin/Views/Country/_Create.cshtml", model), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(CountryCreateViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<CountryTran> countryTrans = new List<CountryTran>();

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

                    CountryTran countryTranAdd = new CountryTran
                    {
                        Name = lang.Name, 
                        LangCode = lang.LangCode
                    };

                    countryTrans.Add(countryTranAdd);
                }

                Country country = new Country
                {
                    Code = model.Code,
                    Image = model.Image,
                    Active = true,
                    Ordering = model.Ordering
                };

                //Country checkCode = countryRepository.GetAll().FirstOrDefault(x=>x.Code.Equals(model.Code));
                //if (checkCode != null) {
                //    return Json(new
                //    {
                //        IsSuccess = false,
                //        Messenger = "Mã đã tồn tại",
                //    }, JsonRequestBehavior.AllowGet);
                //}

                int id = countryRepository.Create(country);

                foreach (var item in countryTrans)
                {
                    item.CountryID = id;
                    countryRepository.Add(item);
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

            List<CountryTran> lstCountryTrans = countryRepository.GetAllCountryTrans().Where(x => x.CountryID == id).ToList();

            Country country = countryRepository.Find(id);

            List<CountryLanguageViewModel> countryLanguageViewModels = new List<CountryLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                CountryTran countryTran_Edit = lstCountryTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (countryTran_Edit != null)
                {
                    CountryLanguageViewModel countryLanguageViewModel = new CountryLanguageViewModel
                    {
                        ID = countryTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Name = countryTran_Edit.Name
                    };
                    countryLanguageViewModels.Add(countryLanguageViewModel);
                }
            }
            var model = new CountryCreateViewModel
            {
                ID = id,
                Code = country.Code,
                Image = country.Image,
                Languages = countryLanguageViewModels
            };
            return Json(RenderViewToString("~/Areas/Admin/Views/Country/_Edit.cshtml", model), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(CountryCreateViewModel model)
        {
            try
            {
                if(string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<CountryTran> countryTrans = new List<CountryTran>();
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

                    CountryTran countryTranEdit = new CountryTran
                    {
                        ID = lang.ID,
                        Name = lang.Name,
                        LangCode = lang.LangCode
                    };
                    countryTrans.Add(countryTranEdit);
                }

                //Country checkCode = countryRepository.GetAll().FirstOrDefault(x => x.Code.Equals(model.Code) && x.ID != model.ID);
                //if (checkCode != null)
                //{
                //    return Json(new
                //    {
                //        IsSuccess = false,
                //        Messenger = "Mã đã tồn tại",
                //    }, JsonRequestBehavior.AllowGet);
                //}

                Country country = new Country();
                country.ID = model.ID;
                country.Code = model.Code;
                country.Image = model.Image;
                country.Ordering = model.Ordering;
                country.Active = model.Active;

                countryRepository.Edit(country);

                foreach (var item in countryTrans)
                {
                    countryRepository.Update(item);
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
        [Authorize(Roles = "Edit")]
        public ActionResult ChangeStatus(int id)
        {
            var obj = countryRepository.Find(id); 
            countryRepository.Edit(obj);
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Thay đổi trạng thái thành công",
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            { 
                countryRepository.Delete(id); 
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

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult DeleteAll(string lstid)
        {
            var arrid = lstid.Split(',');
            var count = 0;
            foreach (var item in arrid)
            {
                try
                {
                    countryRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} bản ghi", count),
            }, JsonRequestBehavior.AllowGet);
        } 
    }
}
