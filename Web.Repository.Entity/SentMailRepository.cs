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
    public class SentMailRepository : ISentMailRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "cacheSentMail"; 
        public void Add(SentMail obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.SentMails.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            HelperCache.RemoveCache(KeyCache);
            var obj = Find(id);
            _entities.SentMails.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(SentMail obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public SentMail Find(int id)
        {
            return _entities.SentMails.Find(id);
        } 

        public IEnumerable<SentMail> FindAll()
        {
            return _entities.SentMails;
        }

        public List<SentMail> GetAll()
        {
            var lstData = HelperCache.GetCache<List<SentMail>>(KeyCache);
            if (lstData == null)
            {
                lstData = _entities.SentMails.ToList();
                HelperCache.AddCache(lstData, KeyCache);
            }
            return lstData;
        }
    }
}
