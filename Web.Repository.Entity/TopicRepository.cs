using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository.Entity
{
    public class TopicRepository : ITopicRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Topic";

        public void Add(Topic model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Topics.Add(model);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Topics.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Topic obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public Topic Find(int id)
        {
            return _entities.Topics.Find(id);
        }
        
        public IEnumerable<Topic> GetAll()
        {
            return _entities.Topics;
        }

    }
}
