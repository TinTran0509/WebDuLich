using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.BaseSecurity
{
    public class GetAction
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        public List<int> Get(string groupUser, int maId)
        {
            var lstAction = new List<int>();
            if (string.IsNullOrEmpty(groupUser)) return lstAction;

            var arrGroupUser = groupUser.Split(',').Select(int.Parse);
            var lstGroupUser = _entities.tbl_GroupUser.Where(g => arrGroupUser.Contains(g.ID) && g.Status).ToList();
            foreach (var objGroupUser in lstGroupUser)
            {
                var lstPermission = HelperXml.DeserializeXml2List<Permission>(objGroupUser.Permission).Where(g => g.AdminMenuId == maId);
                foreach (var permission in lstPermission)
                {
                    if (!string.IsNullOrEmpty(permission.Roles))
                    {
                        var arrAction = permission.Roles.Split(',').Select(int.Parse);
                        lstAction.AddRange(arrAction);    
                    }
                }
            }
            return lstAction.Distinct().ToList();
        }
    }
}