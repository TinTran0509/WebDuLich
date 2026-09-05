using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class IntroductionController : BaseController
    {
        readonly IIntroductionRepository introductionRepository = new IntroductionRepository();
        readonly IIntroductionTransRepository introductionTransRepository = new IntroductionTransRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
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
            var model = introductionRepository.GetAll().ToList(); 
            return Json(new
            { 
                viewContent = RenderViewToString("~/Areas/Admin/Views/Introduction/_ListData.cshtml", model),
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<IntroductionLanguageViewModel> introductionLanguageViewModels = new List<IntroductionLanguageViewModel>();
           
            foreach (var lang in tbl_Languages)
            {
                IntroductionLanguageViewModel introductionLanguageViewModel = new IntroductionLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                introductionLanguageViewModels.Add(introductionLanguageViewModel);
            }
            var model = new IntroductionCreateViewModel
            {
                Languages = introductionLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(IntroductionCreateViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ảnh",
                    }, JsonRequestBehavior.AllowGet);
                }

                List<IntroductionTran> introductionTrans = new List<IntroductionTran>();

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

                    IntroductionTran introductionTranAdd = new IntroductionTran
                    {
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        LangCode = lang.LangCode,
                        Description = lang.Description,
                        Contents = lang.Contents
                    };

                    introductionTrans.Add(introductionTranAdd);
                }

                Introduction introduction = new Introduction
                {
                    Image = model.Image
                };

                int id = introductionRepository.Add(introduction);

                foreach (var item in introductionTrans)
                {
                    item.IntroductionID = id;
                    introductionTransRepository.Add(item);
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

            List<IntroductionTran> lstIntroductionTrans = introductionTransRepository.GetAll().Where(x => x.IntroductionID == id).ToList();

            Introduction introduction = introductionRepository.Find(id);

            List<IntroductionLanguageViewModel> introductionLanguageViewModels = new List<IntroductionLanguageViewModel>();
           
            foreach (var lang in tbl_Languages)
            {
                IntroductionTran productTran_Edit = lstIntroductionTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (productTran_Edit != null)
                {
                    IntroductionLanguageViewModel introductionLanguageViewModel = new IntroductionLanguageViewModel
                    {
                        ID = productTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Title = productTran_Edit.Title,
                        Description = productTran_Edit.Description,
                        Contents = productTran_Edit.Contents,
                    };
                    introductionLanguageViewModels.Add(introductionLanguageViewModel);
                }
            }
            var model = new IntroductionCreateViewModel
            {
                ID = id, 
                Image = introduction.Image,
                Languages = introductionLanguageViewModels
            };

            return View(model);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(IntroductionCreateViewModel model)
        {
            try
            { 
                List<IntroductionTran> introductionTrans = new List<IntroductionTran>();
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

                    IntroductionTran introductionTranEdit = new IntroductionTran
                    {
                        ID = lang.ID,
                        Title = lang.Title,
                        LinkSeo = HelperString.RenderLinkSeo(lang.Title),
                        Description = lang.Description,
                        Contents = lang.Contents
                    };
                    introductionTrans.Add(introductionTranEdit);
                }

                Introduction introduction = new Introduction();
                introduction.ID = model.ID;
                introduction.Image = model.Image;

                introductionRepository.Edit(introduction);

                foreach (var item in introductionTrans)
                {
                    introductionTransRepository.Edit(item);
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
    }
}
