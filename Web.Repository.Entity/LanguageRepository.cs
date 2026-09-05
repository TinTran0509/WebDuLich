using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;

namespace Web.Repository.Entity
{
    public class LanguageRepository : ILanguageRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "cachecategory"; 

        public void Add(tbl_Languages obj)
        {
            _entities.tbl_Languages.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.tbl_Languages.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(tbl_Languages obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public tbl_Languages Find(int id)
        {
            return _entities.tbl_Languages.Find(id);
        }

        public IEnumerable<tbl_Languages> GetAll()
        {
            return _entities.tbl_Languages;
        }

        public IEnumerable<tbl_Languages> GetByActive()
        {
            return _entities.tbl_Languages.Where(x=>x.Active);
        }
    }
}
