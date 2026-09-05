using Excel.Log.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Models;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class BannerController : BaseController
    {
        readonly IBannerRepository bannerRepository = new BannerRepository();
        readonly IMenuRepository menuRepository = new MenuRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        readonly IMenuTransRepository menuTransRepository = new MenuTransRepository(); 
        readonly IBannerTransRepository bannerTransRepository = new BannerTransRepository();
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
            List<BannerModel> bannerModels = new List<BannerModel>();    
            var banners = bannerRepository.GetAll().ToList();
            foreach (var item in banners)
            { 
                List<BannerModel> bannerTrans = bannerTransRepository.GetByBannerID(item.ID).ToList();
                bannerModels.AddRange(bannerTrans);
            }
            var total = 0;
            //model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Banner/_ListData.cshtml", bannerModels),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }  
         
        [HttpGet]
        public ActionResult Create()
        {
            tbl_Languages language = languageRepository.GetAll().FirstOrDefault(x => x.IsDefault);
            if (language != null)
            {
                TempData["Menus"] = menuTransRepository.GetAll().
                    Where(x => x.ParentID != 0 && x.LangCode.Equals(language.LangCode)).ToList();
            }
           
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();
            List<BannerLanguageViewModel> bannerLanguageViewModels = new List<BannerLanguageViewModel>();
            foreach (var lang in tbl_Languages) 
            {
                BannerLanguageViewModel bannerLanguageViewModel = new BannerLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                bannerLanguageViewModels.Add(bannerLanguageViewModel);
            }
            var model = new BannerCreateViewModel
            {
                Languages = bannerLanguageViewModels 
            };

            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(BannerCreateViewModel model)
        {
            try
            {
                //List<BannerTran> lstBannerTrans = bannerTransRepository.GetAll().Where(x => x.BannerID == model.MenuID).ToList();

                if (model.MenuID == 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn chủ đề",
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

                List<BannerTran> bannerTrans = new List<BannerTran>();

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

                    BannerTran bannerTranAdd = new BannerTran
                    {
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Description = lang.Description,
                        Contents = lang.Contents
                    };

                    bannerTrans.Add(bannerTranAdd);
                }

                Banner banner = new Banner
                {
                    Image = model.Image,
                    MenuID = model.MenuID,
                    Active = true
                };

                int id = bannerRepository.Add(banner);

                foreach (var item in bannerTrans)
                {
                    item.BannerID = id;
                    bannerTransRepository.Add(item);
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
            tbl_Languages tbl_Language = tbl_Languages.FirstOrDefault(x => x.IsDefault);
            TempData["Menus"] = menuTransRepository.GetAll().Where(x => x.ParentID != 0 && x.LangCode.Equals(tbl_Language.LangCode)).ToList();

            List<BannerTran> lstBannerTrans = bannerTransRepository.GetAll().Where(x => x.BannerID == id).ToList();

            Banner banner = bannerRepository.Find(id);

            List<BannerLanguageViewModel> bannerLanguageViewModels = new List<BannerLanguageViewModel>();
            foreach (var lang in tbl_Languages)
            {
                BannerTran bannerTran_Edit = lstBannerTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (bannerTran_Edit != null)
                {
                    BannerLanguageViewModel bannerLanguageViewModel = new BannerLanguageViewModel
                    {
                        ID = bannerTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = bannerTran_Edit.Title,
                        Description = bannerTran_Edit.Description,
                        Contents = bannerTran_Edit.Contents,
                    };
                    bannerLanguageViewModels.Add(bannerLanguageViewModel);
                }
            }
            var model = new BannerCreateViewModel
            {
                ID = id,
                MenuID = banner.MenuID,
                Active = banner.Active,
                Image = banner.Image,
                CreatedDate = banner.CreatedDate,
                Languages = bannerLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BannerCreateViewModel model)
        {
            try
            { 
                List<BannerTran> lstBannerTrans = bannerTransRepository.GetAll().Where(x => x.BannerID == model.ID).ToList();

                List<BannerTran> bannerTrans = new List<BannerTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Title))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm tiêu đề " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }
                     
                    BannerTran bannerTranEdit = new BannerTran
                    {
                        ID = lang.ID, 
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        Description = lang.Description,
                        Contents = lang.Contents
                    };
                    bannerTrans.Add(bannerTranEdit);
                }

                foreach (var item in bannerTrans)
                { 
                    bannerTransRepository.Edit(item);
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
                List<BannerTran> bannerTranList = bannerTransRepository.GetAll().Where(m => m.BannerID == id).ToList();
                bannerRepository.Delete(id, bannerTranList);
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
