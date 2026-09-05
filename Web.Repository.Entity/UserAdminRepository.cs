using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository.Entity
{
    public class UserAdminRepository : IUserAdminRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        public void Add(UserAdmin obj)
        {
            _entities.UserAdmins.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.UserAdmins.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(UserAdmin obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }
        public void Edit(List<UserAdmin> lstobj)
        {
            foreach (var obj in lstobj)
            {
                _entities.Entry(obj).State = EntityState.Modified;
            }
            _entities.SaveChanges();
        }
        public UserAdmin Find(int id)
        {
            return _entities.UserAdmins.Find(id);
        }

        public IEnumerable<UserAdmin> GetAll()
        {
            return _entities.UserAdmins;
        }
    }
}
