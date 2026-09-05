using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;
using Web.Controllers;

namespace Web.Areas.Admin.Controllers
{
    public class GroupUserController : BaseController
    {
        private readonly IGroupUserRepository _groupUserRepository = new GroupUserRepository();
        private readonly IAdminMenuRepository _adminMenuRepository = new AdminMenuRepository();
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();
        private readonly IUserAdminRepository _userRepository = new UserAdminRepository();

        //
        // GET: /Category/
        [Authorize(Roles = "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Index")]
        public ActionResult ListData(int page = 1)
        {
            var lstGroupUser = _groupUserRepository.GetAll();
            var totalRecord = lstGroupUser.Count();
            lstGroupUser =
                _groupUserRepository.GetAll().Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/GroupUser/_ListData.cshtml", lstGroupUser),
                totalPages = Math.Ceiling(((double)totalRecord / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        public ActionResult Add()
        {
            return Json(RenderViewToString("~/Areas/Admin/Views/GroupUser/_Create.cshtml"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Add")]
        public ActionResult Add(tbl_GroupUser obj)
        {
            try
            {
                _groupUserRepository.Add(obj);
               
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới nhóm thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới nhóm thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int id)
        {
            var objGroupUser = _groupUserRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/GroupUser/_Edit.cshtml", objGroupUser), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult Edit(tbl_GroupUser obj)
        {
            try
            {
                var objOld = _groupUserRepository.Find(obj.ID);
                objOld.Name = obj.Name;
                objOld.Status = obj.Status;
                objOld.ExpandNews = obj.ExpandNews;
                _groupUserRepository.Edit(objOld);
               
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật danh mục thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            /*catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Cập nhật danh mục thất bại")
                }, JsonRequestBehavior.AllowGet);
            }*/
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var obj = _groupUserRepository.Find(id);
                _groupUserRepository.Delete(id);
               
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa danh mục thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa danh mục thành công",
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
                    _groupUserRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} danh mục", count),
            }, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Index")]
        [HttpPost]
        public ActionResult Search(tbl_GroupUser obj, int page = 1)
        {
            var lstGroupUser = _groupUserRepository.GetAll();
            if (!string.IsNullOrEmpty(obj.Name))
            {
                lstGroupUser =
                    lstGroupUser.Where(
                        g =>
                            HelperString.UnsignCharacter(g.Name.ToLower().Trim())
                                .Contains(HelperString.UnsignCharacter(obj.Name).ToLower().Trim()));
            }
            var totalRecord = lstGroupUser.Count();
            lstGroupUser =
                lstGroupUser.Skip((page - 1) * Webconfig.RowLimit).Take(Webconfig.RowLimit).ToList();
            return Json(new
            {
                viewContent = RenderViewToString("~/Areas/Admin/Views/GroupUser/_ListData.cshtml", lstGroupUser),
                totalPages = Math.Ceiling(((double)totalRecord / Webconfig.RowLimit)),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Edit")]
        public ActionResult Permission(int id)
        {
            var lstOrder = new List<AdminMenu>();
            var lstAdminMenu = _adminMenuRepository.GetAll().ToList();
            var lstParents = lstAdminMenu.Where(g => g.ParentID == 0).OrderBy(g => g.Ordering).ToList();
            foreach (var tblAdminMenu in lstParents)
            {
                lstOrder.Add(tblAdminMenu);
                var lstChild = lstAdminMenu.Where(g => g.ParentID == tblAdminMenu.ID).OrderBy(g => g.Ordering).ToList();
                if (lstChild.Count > 0)
                {
                    lstOrder.AddRange(lstChild);
                }
            }
            lstOrder = Common.CreateLevel(lstOrder);
            TempData["GroupUser"] = _groupUserRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/GroupUser/_Permission.cshtml", lstOrder),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult Permission(int groupuserId, string roles)
        {
            var objGroupUser = _groupUserRepository.Find(groupuserId);
            var arrRoles = roles.Split(':');
            var lstPermission = HelperXml.DeserializeXml2List<Permission>(objGroupUser.Permission);
            var objPermission = lstPermission.FirstOrDefault(g => g.AdminMenuId == Convert.ToInt32(arrRoles[0]));

            if (objPermission == null)
            {
                objPermission = new Permission
                {
                    Roles = arrRoles[1],
                    AdminMenuId = Convert.ToInt32(roles.Split(':')[0])
                };
                lstPermission.Add(objPermission);
            }
            else
            {
                if (arrRoles.Length < 2)
                {
                    lstPermission.Remove(objPermission);
                }
                else
                {
                    objPermission.Roles = arrRoles[1];
                }
            }
            objGroupUser.Permission = HelperXml.SerializeList2Xml(lstPermission);
            try
            {
                _groupUserRepository.Edit(objGroupUser);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = string.Format("Phân quyền cho nhóm thành công")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Phân quyền thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Edit")]
        public ActionResult PermissionCatNews(int id)
        {
            var catsAdd = _categoryRepository.GetAll().ToList();
            var lstCat = Common.CreateLevel(catsAdd);
            TempData["GroupUser"] = _groupUserRepository.Find(id);
            return Json(RenderViewToString("~/Areas/Admin/Views/GroupUser/_PermissionCatNews.cshtml", lstCat),
                JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult PermissionCatNews(int groupuserId, string roles)
        {
            var objGroupUser = _groupUserRepository.Find(groupuserId);
            var arrRoles = roles.Split(':');
            var lstPermission = HelperXml.DeserializeXml2List<Permission>(objGroupUser.PermissionCatNews);
            var objPermission = lstPermission.FirstOrDefault(g => g.AdminMenuId == Convert.ToInt32(arrRoles[0]));

            if (objPermission == null)
            {
                objPermission = new Permission
                {
                    Roles = arrRoles[1],
                    AdminMenuId = Convert.ToInt32(roles.Split(':')[0])
                };
                lstPermission.Add(objPermission);
            }
            else
            {
                if (arrRoles.Length < 2)
                {
                    lstPermission.Remove(objPermission);
                }
                else
                {
                    objPermission.Roles = arrRoles[1];
                }
            }
            objGroupUser.PermissionCatNews = HelperXml.SerializeList2Xml(lstPermission);
            try
            {
                _groupUserRepository.Edit(objGroupUser);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = string.Format("Phân quyền cho nhóm thành công")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Phân quyền thất bại")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Add")]
        public ActionResult AddUserToGroup(int id)
        {
            var objGroupUser = _groupUserRepository.Find(id);
            var lstUser = _userRepository.GetAll().Where(g => g.Active).ToList();
            //lấy những user đã được thêm vào nhóm
            var lstUserAdded =
                lstUser.Where(
                    g =>
                        !string.IsNullOrEmpty(g.GroupUserID) &&
                        g.GroupUserID.Split(',').Select(Int32.Parse).Contains(id)).ToList();
            //danh sách user chưa được thêm vào nhóm
            var lstUserUnAdd =
                lstUser.Where(
                    g =>
                        lstUserAdded.Count(t => t.ID == g.ID) == 0 && g.UserType != (int)EnumHelper.UserType.Binhthuong)
                    .ToList();
            TempData["lstUserAdded"] = lstUserAdded;
            TempData["lstUserUnAdd"] = lstUserUnAdd;
            return Json(RenderViewToString("~/Areas/Admin/Views/GroupUser/_AddUserToGroup.cshtml", objGroupUser),
                JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult AddUserToGroup(int[] to, int id)
        {
            var lstUser = _userRepository.GetAll().ToList();
            //lấy những user đã được thêm vào nhóm
            var lstUserAdded = lstUser.Where(g => g.Active && !string.IsNullOrEmpty(g.GroupUserID) && g.GroupUserID.Split(',').Select(Int32.Parse).Contains(id)).ToList();
            //xóa hết user khỏi nhóm
            foreach (var userAdded in lstUserAdded)
            {
                var arrUser = userAdded.GroupUserID.Split(',').Select(Int32.Parse).Where(val => val != id).ToArray();
                userAdded.GroupUserID = string.Join(",", arrUser);
            }
            try
            {
                _userRepository.Edit(lstUserAdded);
            }
            catch (Exception)
            {
                return Json(new
                 {
                     IsSuccess = false,
                     Messenger = string.Format("Có lỗi xảy ra")
                 }, JsonRequestBehavior.AllowGet);
            }

            //thêm lại danh sách user vào nhóm
            if (to != null)
            {
                var lstUserNewAdd = lstUser.Where(g => to.Contains(g.ID)).ToList();
                foreach (var user in lstUserNewAdd)
                {
                    var arrUser = new List<int>();
                    if (!string.IsNullOrEmpty(user.GroupUserID))
                    {
                        arrUser = user.GroupUserID.Split(',').Select(Int32.Parse).ToList();
                    }
                    arrUser.Add(id);
                    user.GroupUserID = string.Join(",", arrUser);
                }
                try
                {
                    _userRepository.Edit(lstUserNewAdd);
                }
                catch (Exception)
                {
                    return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Có lỗi xảy ra")
                }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new
                {
                    IsSuccess = true,
                    Messenger = string.Format("Thêm người dùng vào nhóm thành công")
                }, JsonRequestBehavior.AllowGet);
        }
    }
}
