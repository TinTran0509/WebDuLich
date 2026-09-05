using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Providers.Entities;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.BaseSecurity
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            if (UserType == (int)EnumHelper.UserType.Admin)
            {
                var lstFullAction = Enum.GetValues(typeof(EnumHelper.Action))
                                    .Cast<int>()
                                    .ToList();
                HttpContext.Current.Session["Action"] = lstFullAction;
                return true;
            }
            if (role.ToLower() == "noibo" && NoiBo == 1) return true;
            var action = new GetAction();
            var chucnangid = "";
            if (string.IsNullOrEmpty(chucnangid))
            {
                var httpCookie = HttpContext.Current.Request.Cookies["chucnangid"];
                if (httpCookie != null)
                    chucnangid = httpCookie.Value;
            }
            if (string.IsNullOrEmpty(chucnangid))
            {
                HttpContext.Current.Response.Redirect("/Error/AccessDenined");
            }
            List<int> lstAction = action.Get(GroupUser, Convert.ToInt32(chucnangid));
            if (lstAction.Count == 0)
            {
                HttpContext.Current.Response.Redirect("/Error/AccessDenined");
            }
            else
            {
                //var actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();

                bool hasPermision = false;
                switch (role.ToLower())
                {
                    case "index":
                        if (lstAction.Contains((int)EnumHelper.Action.View))
                        {
                            hasPermision = true;
                        }
                        break;
                    case "add":
                    case "insert":
                        if (lstAction.Contains((int)EnumHelper.Action.Add))
                        {
                            hasPermision = true;
                        }
                        break;
                    case "edit":
                    case "update":
                        if (lstAction.Contains((int)EnumHelper.Action.Edit))
                        {
                            hasPermision = true;
                        }
                        break;
                    case "delete":
                        if (lstAction.Contains((int)EnumHelper.Action.Delete))
                        {
                            hasPermision = true;
                        }
                        break;
                    case "approved":
                        if (lstAction.Contains((int)EnumHelper.Action.Approved))
                        {
                            hasPermision = true;
                        }
                        break;
                    case "expandnews":
                        // kiem tra xem co duoc mo rong tin khong
                        var _groupuserRepository = new GroupUserRepository();
                        if (GroupUser != null)
                        {
                            var arrGroupUser = GroupUser.Split(',');
                            int isExpandNews = 0;
                            for (int i = 0; i < arrGroupUser.Count(); i++)
                            {
                                int GroupUserID = Convert.ToInt32(arrGroupUser[i]);
                                var rowGroupUser = _groupuserRepository.Find(GroupUserID);
                                if (rowGroupUser != null)
                                {
                                    bool ExpandNews = rowGroupUser.ExpandNews;
                                    if (ExpandNews)
                                    {
                                        isExpandNews = 1;
                                    }
                                }
                            }
                            if (isExpandNews == 1)
                            {
                                hasPermision = true;
                            }
                        }
                        //
                        break;
                    default:
                        if (lstAction.Contains((int)EnumHelper.Action.View))
                        {
                            hasPermision = true;
                        }
                        break;
                }
                if (hasPermision == false)
                {
                    HttpContext.Current.Response.Redirect("/Error/AccessDenined");
                }
                HttpContext.Current.Session["Action"] = lstAction;
            }
            return true;
        }

        public CustomPrincipal(string UserName)
        {
            this.Identity = new GenericIdentity(UserName);
        }

        public int ID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public string GroupUser { get; set; }
        public byte? NoiBo { get; set; }
        public bool isAdmin { get; set; }
    }
}