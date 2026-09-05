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
    public class TopicTransRepository : ITopicTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "TopicTrans";

        public void Add(TopicTran model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.TopicTrans.Add(model);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.TopicTrans.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(TopicTran obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(TopicTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE TopicTrans SET Title = @Title, LinkSeo = @LinkSeo, Description = @Description,");
                    sb.Append("Content = @Content, LangCode = @LangCode WHERE ID = @ID"); 

                    var parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Title", obj.Title);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("Description", obj.Description);
                    parameters.Add("Contents", obj.Contents);
                    parameters.Add("LangCode", obj.LangCode);

                    conn.Execute(
                        sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TopicTran Find(int id)
        {
            return _entities.TopicTrans.Find(id);
        }
        
        public IEnumerable<TopicTran> GetAll()
        {
            return _entities.TopicTrans;
        }

    }
}
