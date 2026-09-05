using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;

namespace Web.Repository.Entity
{
    public class AdminMenuRepository : IAdminMenuRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "cacheadminmenu"; 
        public void Add(AdminMenu obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.AdminMenus.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            HelperCache.RemoveCache(KeyCache);
            var obj = Find(id);
            _entities.AdminMenus.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(AdminMenu obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public AdminMenu Find(int id)
        {
            return _entities.AdminMenus.Find(id);
        }

        public List<AdminMenu> GetAll()
        {
            var lstData = HelperCache.GetCache<List<AdminMenu>>(KeyCache);
            if (lstData == null)
            {
                lstData = _entities.AdminMenus.ToList();
                HelperCache.AddCache(lstData, KeyCache);
            }
            return lstData;
        }
    }
}
