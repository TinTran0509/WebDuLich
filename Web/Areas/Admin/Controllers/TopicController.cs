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
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class TopicController : BaseController
    {
        readonly ITopicRepository topicRepository = new TopicRepository();
        readonly IMenuRepository menuRepository = new MenuRepository();
        readonly ILanguageRepository languageRepository = new LanguageRepository();
        readonly IMenuTransRepository menuTransRepository = new MenuTransRepository(); 
        readonly ITopicTransRepository topicTransRepository = new TopicTransRepository();
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
            var model = topicRepository.GetAll().ToList();
            var total = model.Count();
            model = model.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Topic/_ListData.cshtml", model),
                totalPages = Math.Ceiling(((double)total / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        } 

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Topic/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(Topic obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh",
                    }, JsonRequestBehavior.AllowGet);
                }
                topicRepository.Add(obj);
                
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
            Topic topic = topicRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Topic/_Edit.cshtml", topic), JsonRequestBehavior.AllowGet);
        }  

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(Topic obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Image))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng thêm ảnh",
                    }, JsonRequestBehavior.AllowGet);
                }
                topicRepository.Edit(obj);
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
                topicRepository.Delete(id);
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

        public ActionResult GetMenuTransByLangCode(string langCode)
        {
            var menus = new List<MenuTran>();
            var lstMenus = menuTransRepository.GetByLangCode(langCode).ToList();
            var lstParents = lstMenus.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var parent in lstParents)
                { 
                    menus.Add(parent);
                    var lstChild = lstMenus.Where(g => g.ParentID == parent.ID).OrderBy(g => g.Ordering).ToList();
                    if (lstChild.Count > 0)
                    {
                        foreach (var item in lstChild)
                        {
                            item.Name = "-- " + item.Name;
                            menus.Add(item);
                        }
                    }
                }
            }
            else
            {
                menus.AddRange(lstMenus);
            }

            return Json(new
            { 
                Data = menus
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult AddTrans(int id)
        {
            var languages = languageRepository.GetByActive().ToList();
            TempData["Languages"] = languages;
            TopicTran topicTran = new TopicTran();
            topicTran.TopicID = id;
            return View(topicTran);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult AddTrans(TopicTran obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                TopicTran topicTran = topicTransRepository.GetAll().Where(x=>x.TopicID == obj.TopicID && x.LangCode.Equals(obj.LangCode)).FirstOrDefault();
                if (topicTran != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Bản dịch cho ngôn ngữ " + obj.LangCode +" đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.LinkSeo)) 
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn chủ đề",
                    }, JsonRequestBehavior.AllowGet);
                }
                topicTransRepository.Add(obj);

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
        public ActionResult EditTrans(int id)
        { 
            TempData["Languages"] = languageRepository.GetByActive().ToList(); 
            TopicTran topicTran = topicTransRepository.Find(id);
            if (topicTran != null) 
            {
                List<MenuTran> menus = new List<MenuTran>();
                var lstMenus = menuTransRepository.GetByLangCode(topicTran.LangCode).ToList();
                var lstParents = lstMenus.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
                if (lstParents.Count > 0)
                {
                    foreach (var parent in lstParents)
                    {
                        menus.Add(parent);
                        var lstChild = lstMenus.Where(g => g.ParentID == parent.ID).OrderBy(g => g.Ordering).ToList();
                        if (lstChild.Count > 0)
                        {
                            foreach (var item in lstChild)
                            {
                                item.Name = "-- " + item.Name;
                                menus.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    menus.AddRange(lstMenus);
                }
                TempData["MenuTrans"] = menus;
            }
            else
            {
                TempData["MenuTrans"] = null;
            }
            return View(topicTran);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult EditTrans(TopicTran obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.LangCode))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn ngôn ngữ",
                    }, JsonRequestBehavior.AllowGet);
                }
                TopicTran topicTran = topicTransRepository.GetAll().Where(x => x.TopicID == obj.TopicID && x.LangCode.Equals(obj.LangCode)
                && x.ID != obj.ID).FirstOrDefault();
                if (topicTran != null)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Bản dịch cho ngôn ngữ " + obj.LangCode + " đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(obj.LinkSeo))
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Vui lòng chọn chủ đề",
                    }, JsonRequestBehavior.AllowGet);
                }
                topicTransRepository.Update(obj);

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

        public ActionResult Translate(int id)
        {
            var topicTrans = topicTransRepository.GetAll().Where(x=>x.TopicID == id).ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/Topic/_Translate.cshtml", topicTrans), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult DeleteTopicTrans(int id)
        {
            try
            {
                topicTransRepository.Delete(id);
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
