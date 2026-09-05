using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class SliderController : BaseController
    {
        readonly ISliderRepository _slideImagesRepository = new SliderRepository();
        readonly IUserAdminRepository _userAdminRepository = new UserAdminRepository();
        //
        // GET: /Admin/SliderImages/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            var objUser = _userAdminRepository.Find(User.ID);
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
            var lstSlideImages = _slideImagesRepository.GetAll().OrderBy(g=>g.DisplayOrder).ToList();
            var totalSlideImages = lstSlideImages.Count();
            lstSlideImages = lstSlideImages.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Slider/_ListData.cshtml", lstSlideImages),
                totalPages = Math.Ceiling(((double)totalSlideImages / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Slider/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(Slider obj)
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
                _slideImagesRepository.Add(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới ảnh slide thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới ảnh slider thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int id)
        {
            var objSlideImages = _slideImagesRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Slider/_Edit.cshtml", objSlideImages), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(Slider obj)
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
                _slideImagesRepository.Edit(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật slider ảnh thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Cập nhật slider ảnh thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Edit")]
        public ActionResult ChangeStatus(int id)
        {
            var obj = _slideImagesRepository.Find(id);
            obj.Status = !obj.Status;
            _slideImagesRepository.Edit(obj);
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
                var obj = _slideImagesRepository.Find(id);
                _slideImagesRepository.Delete(id);
               
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa slider ảnh thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa slide ảnh thành công",
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
                    _slideImagesRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} slider ảnh", count),
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult UpdatePosition(string value)
        {
            var arrValue = value.Split('|');
            foreach (var item in arrValue)
            {
                var id = item.Split(':')[0];
                var pos = item.Split(':')[1];
                var obj = _slideImagesRepository.Find(Convert.ToInt32(id));
                obj.DisplayOrder = Convert.ToInt32(pos);
                try
                {
                    _slideImagesRepository.Edit(obj);
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = string.Format("Cập nhật vị trí thất bại")
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Cập nhật vị trí thành công",
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
