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
    public class ParamRepository : IParamRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "cacheParam"; 
        public void Add(Param obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Params.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            HelperCache.RemoveCache(KeyCache);
            var obj = Find(id);
            _entities.Params.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Param obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public Param Find(int id)
        {
            return _entities.Params.Find(id);
        }

        public Param FindByKey(string key)
        {
            return _entities.Params.Where(x=>x.KeyName == key).FirstOrDefault();
        }

        public List<Param> GetAll()
        {
            var lstData = HelperCache.GetCache<List<Param>>(KeyCache);
            if (lstData == null)
            {
                lstData = _entities.Params.ToList();
                HelperCache.AddCache(lstData, KeyCache);
            }
            return lstData;
        }
    }
}
