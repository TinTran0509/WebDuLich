using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Controllers;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class ParamController : BaseController
    {
        private readonly IParamRepository _paramRepository = new ParamRepository();
        const string Keycache = "KeyParam";
        //
        // GET: /Param/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int page = 1)
        {
            var lstParam = _paramRepository.GetAll().ToList();            
                      
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Param/_ListData.cshtml", lstParam),
            }, JsonRequestBehavior.AllowGet);
        }
  
        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Param/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(Param obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj.KeyName))
                {
                    Param param = _paramRepository.FindByKey(obj.KeyName);
                    if (param != null)
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Messenger = "Tên tham số đã tồn tại",
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                _paramRepository.Add(obj);
                HelperCache.RemoveCache(Keycache);
                
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
            var obj = _paramRepository.Find(id); 
            return Json(RenderViewToString("~/Areas/Admin/Views/Param/_Edit.cshtml", obj), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(Param obj)
        {
            try
            {
                Param param = _paramRepository.FindByKey(obj.KeyName);
                if (param != null && param.ID != obj.ID)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên tham số đã tồn tại",
                    }, JsonRequestBehavior.AllowGet);
                }
                _paramRepository.Edit(obj);
                HelperCache.RemoveCache(Keycache);
                
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
                var obj = _paramRepository.Find(id);
                _paramRepository.Delete(id);
                HelperCache.RemoveCache(Keycache);
               
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
            HelperCache.RemoveCache(Keycache);
            foreach (var item in arrid)
            {
                try
                {
                    _paramRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} menu", count),
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
