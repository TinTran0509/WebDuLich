using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository.Entity
{
    public class GroupUserRepository : IGroupUserRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        public void Add(Model.tbl_GroupUser obj)
        {
            _entities.tbl_GroupUser.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.tbl_GroupUser.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Model.tbl_GroupUser obj)
        {
            _entities.Entry(obj).State = EntityState.Detached;
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }
        
        public Model.tbl_GroupUser Find(int id)
        {
            return _entities.tbl_GroupUser.Find(id);
        }

        public IEnumerable<Model.tbl_GroupUser> GetAll()
        {
            return _entities.tbl_GroupUser;
        }
    }
}
