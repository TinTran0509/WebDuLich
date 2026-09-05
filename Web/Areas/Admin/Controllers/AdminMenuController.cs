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
    public class AdminMenuController : BaseController
    {
        readonly IAdminMenuRepository _adminMenuRepository = new AdminMenuRepository();
        readonly IGroupUserRepository _groupUserRepository = new GroupUserRepository();
        const string Keycache = "keyadminmenu";
        //
        // GET: /AdminMenu/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int page = 1)
        {
            var lstAdminMenu = _adminMenuRepository.GetAll();            
            foreach (var item in lstAdminMenu)
            {
                var objParent = lstAdminMenu.FirstOrDefault(g => g.ID == item.ParentID);
                if (objParent != null)
                {
                    item.ParentName = objParent.Name;
                }
            }
            var lstLevel = Common.CreateLevel(lstAdminMenu.ToList());            
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/AdminMenu/_ListData.cshtml", lstLevel),
            }, JsonRequestBehavior.AllowGet);
        }

        readonly List<AdminMenu> _lstOtherAdminMenus = new List<AdminMenu>();

        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult Search(AdminMenu obj)
        {
            var lstAdminMenu = _adminMenuRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(obj.Name))
            {
                lstAdminMenu =
                    lstAdminMenu.Where(
                        g =>
                            HelperString.UnsignCharacter(g.Name.Trim().ToLower()).Contains(HelperString.UnsignCharacter(obj.Name.ToLower().Trim()))).ToList();
                foreach (var cat in lstAdminMenu)
                {
                    AddParent(cat, lstAdminMenu, _adminMenuRepository.GetAll().ToList());
                }
                lstAdminMenu.AddRange(_lstOtherAdminMenus);
            }
            foreach (var item in lstAdminMenu)
            {
                var objParent = lstAdminMenu.FirstOrDefault(g => g.ID == item.ParentID);
                if (objParent != null)
                {
                    item.Name = objParent.Name;
                }
            }
            var lstLevel = Common.CreateLevel(lstAdminMenu.ToList());
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/AdminMenu/_ListData.cshtml", lstLevel),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            var lstAdminMenu = Common.CreateLevel(_adminMenuRepository.GetAll().ToList());
            TempData["AdminMenu"] = lstAdminMenu.ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/AdminMenu/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult Add(AdminMenu obj)
        {
            try
            {
                _adminMenuRepository.Add(obj);
                HelperCache.RemoveCache(Keycache);
                
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới menu thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới menu thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [Authorize(Roles = "Edit")]
        public ActionResult ChangeStatus(int id)
        {
            var obj = _adminMenuRepository.Find(id);
            obj.Active = !obj.Active;
            _adminMenuRepository.Edit(obj);
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Thay đổi trạng thái thành công",
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int id)
        {
            var objAdminMenu = _adminMenuRepository.Find(id);
            var lstAdminMenu = Common.CreateLevel(_adminMenuRepository.GetAll().ToList());
            TempData["AdminMenu"] = lstAdminMenu.ToList();
            return Json(RenderViewToString("~/Areas/Admin/Views/AdminMenu/_Edit.cshtml", objAdminMenu), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(AdminMenu obj)
        {
            try
            {
                _adminMenuRepository.Edit(obj);
                HelperCache.RemoveCache(Keycache);
                
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật menu thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Cập nhật menu thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var obj = _adminMenuRepository.Find(id);
                _adminMenuRepository.Delete(id);
                HelperCache.RemoveCache(Keycache);
               
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa menu thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa menu thành công",
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
                    _adminMenuRepository.Delete(Convert.ToInt32(item));
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

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult UpdatePosition(string value)
        {
            var arrValue = value.Split('|');
            HelperCache.RemoveCache(Keycache);
            foreach (var item in arrValue)
            {
                var id = item.Split(':')[0];
                var pos = item.Split(':')[1];
                var obj = _adminMenuRepository.Find(Convert.ToInt32(id));
                obj.Ordering = Convert.ToInt32(pos);
                try
                {
                    _adminMenuRepository.Edit(obj);

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
        public ActionResult MenuLeft()
        {
            var lstMenuAdmin = new List<AdminMenu>(); 
            if (!string.IsNullOrEmpty(User.GroupUser))
            {
                lstMenuAdmin = _adminMenuRepository.GetAll().Where(x => x.Active).ToList();
                var arrGroupUser = User.GroupUser.Split(',').Select(Int32.Parse);
                var lstChucNangId = new List<int>();
                foreach (var groupUser in arrGroupUser)
                {
                    var objGroupUser = _groupUserRepository.Find(groupUser);
                    var perrmission = HelperXml.DeserializeXml2List<Permission>(objGroupUser.Permission);
                    lstChucNangId.AddRange(perrmission.Select(g => g.AdminMenuId));
                }
                if (lstChucNangId.Count > 0)
                {
                    lstMenuAdmin = lstMenuAdmin.Where(g => lstChucNangId.Contains(g.ID)).ToList();
                }
            }
            return Json(RenderViewToString("~/Areas/Admin/Views/AdminMenu/_MenuLeft.cshtml", lstMenuAdmin), JsonRequestBehavior.AllowGet);
        }
        //Trong trường hợp nếu category tìm kiếm mà có parent thì add thêm parent vào cho nó
        readonly List<int> _lstDic = new List<int>();

        public void AddParent(AdminMenu cat, List<AdminMenu> lstaAdminMenus, List<AdminMenu> lstAllAdminMenus)
        {
            if (_lstDic.Contains(cat.ID) || cat.ParentID == 0) return;
            _lstDic.Add(cat.ID);
            var parent = lstaAdminMenus.Find(g => g.ID == cat.ParentID);
            //Nếu không có
            if (parent != null) return;
            parent = lstAllAdminMenus.Find(g => g.ID == cat.ParentID);
            _lstOtherAdminMenus.Add(parent);
            AddParent(parent, lstaAdminMenus, lstAllAdminMenus);
        }
    }
}
