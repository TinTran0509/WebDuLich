using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Areas.Admin.Controllers
{
    public class PackageController : BaseController
    {
        readonly IPackageRepository _packageRepository = new PackageRepository();
        //
        // GET: /Admin/Package/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            List<Package_Price> package_Prices = _packageRepository.GetAllPackagePrice().ToList();
            TempData["Package3"] = package_Prices.Where(x => x.PackageID == 3).ToList();
            TempData["Package4"] = package_Prices.Where(x => x.PackageID == 4).ToList();
            TempData["Package5"] = package_Prices.Where(x => x.PackageID == 5).ToList();
            TempData["Package6"] = package_Prices.Where(x => x.PackageID == 6).ToList();
            return View();
        }

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult ListData(int page = 1)
        {
            var lstSlideImages = _packageRepository.GetAll().OrderBy(g=>g.Ordering).ToList();
            var totalSlideImages = lstSlideImages.Count();
            lstSlideImages = lstSlideImages.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/Package/_ListData.cshtml", lstSlideImages),
                totalPages = Math.Ceiling(((double)totalSlideImages / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/Package/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(Package obj)
        {
            try
            { 
                _packageRepository.Add(obj);
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
            var objSlideImages = _packageRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/Package/_Edit.cshtml", objSlideImages), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(Package obj)
        {
            try
            { 
                _packageRepository.Edit(obj);
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
                var obj = _packageRepository.Find(id);
                _packageRepository.Delete(id);
               
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
                    _packageRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} package", count),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult SavePrice(string packages)
        {
            try
            {
                List<Package_Price> package_Prices = new List<Package_Price>();
                JavaScriptSerializer json = new JavaScriptSerializer();

                if (!string.IsNullOrEmpty(packages))
                {
                    _packageRepository.TrunCatePackage();
                    package_Prices = json.Deserialize<List<Package_Price>>(packages);
                }

                foreach (Package_Price package in package_Prices)
                {
                    _packageRepository.AddPackagePrice(package);
                }

                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Lưu thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Lưu thất bại",
                }, JsonRequestBehavior.AllowGet);
            } 
        }
    }
}
