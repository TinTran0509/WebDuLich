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
    public class WordController : BaseController
    {
        readonly IWordRepository wordRepository = new WordRepository();
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
            var model = wordRepository.GetAll().OrderBy(x=>x.ID).ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Word/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            List<tbl_Languages> tbl_Languages = languageRepository.GetByActive().ToList();

            List<WordLanguageViewModel> wordLanguageViewModels = new List<WordLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                WordLanguageViewModel wordLanguageViewModel = new WordLanguageViewModel
                {
                    LangCode = lang.LangCode,
                    LangName = lang.LangName
                };
                wordLanguageViewModels.Add(wordLanguageViewModel);
            }
            var model = new WordCreateViewModel
            {
                Languages = wordLanguageViewModels
            };
            return Json(RenderViewToString("~/Areas/Admin/Views/Word/_Create.cshtml", model), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(WordCreateViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.KeyName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập từ khóa"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<WordTran> wordTrans = new List<WordTran>();

                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Value))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm bản dịch " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    WordTran wordTranAdd = new WordTran
                    {
                        Value = lang.Value, 
                        LangCode = lang.LangCode
                    };

                    wordTrans.Add(wordTranAdd);
                }

                Word word = new Word
                {
                    KeyName = model.KeyName, 
                    Active = true
                };

                int id = wordRepository.Create(word);

                foreach (var item in wordTrans)
                {
                    item.WordID = id;
                    wordRepository.Add(item);
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

            List<WordTran> lstWordTrans = wordRepository.GetAllWordTrans().Where(x => x.WordID == id).ToList();

            Word word = wordRepository.Find(id);

            List<WordLanguageViewModel> wordLanguageViewModels = new List<WordLanguageViewModel>();

            foreach (var lang in tbl_Languages)
            {
                WordTran wordTran_Edit = lstWordTrans.FirstOrDefault(m => m.LangCode == lang.LangCode);

                if (wordTran_Edit != null)
                {
                    WordLanguageViewModel wordLanguageViewModel = new WordLanguageViewModel
                    {
                        ID = wordTran_Edit.ID,
                        LangCode = lang.LangCode,
                        LangName = lang.LangName,
                        Value = wordTran_Edit.Value
                    };
                    wordLanguageViewModels.Add(wordLanguageViewModel);
                }
            }
            var model = new WordCreateViewModel
            {
                ID = id,
                KeyName = word.KeyName, 
                Languages = wordLanguageViewModels
            };
            return Json(RenderViewToString("~/Areas/Admin/Views/Word/_Edit.cshtml", model), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(WordCreateViewModel model)
        {
            try
            {
                if(string.IsNullOrEmpty(model.KeyName))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng nhập từ khóa"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<WordTran> wordTrans = new List<WordTran>();
                foreach (var lang in model.Languages)
                {
                    if (string.IsNullOrEmpty(lang.Value))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Vui lòng thêm bản dịch " + lang.LangName,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    WordTran wordTranEdit = new WordTran
                    {
                        ID = lang.ID,
                        Value = lang.Value,
                        LangCode = lang.LangCode
                    };
                    wordTrans.Add(wordTranEdit);
                } 

                Word word = new Word();
                word.ID = model.ID;
                word.KeyName = model.KeyName;
                word.Active = true;

                wordRepository.Edit(word);

                foreach (var item in wordTrans)
                {
                    wordRepository.Update(item);
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
            var obj = wordRepository.Find(id);
            wordRepository.Edit(obj);
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
                wordRepository.Delete(id); 
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
                    wordRepository.Delete(Convert.ToInt32(item));
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
