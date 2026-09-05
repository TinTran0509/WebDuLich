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
    public class BannerTransRepository : IBannerTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "BannerTrans";

        public void Add(BannerTran model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.BannerTrans.Add(model);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.BannerTrans.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(BannerTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE BannerTrans ");
                    sb.Append("SET Title = @Title,LinkSeo = @LinkSeo, ");
                    sb.Append("Description = @Description,Contents = @Contents ");
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Title", obj.Title);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("Description", obj.Description);
                    parameters.Add("Contents", obj.Contents); 
                    conn.Execute(sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(BannerTran obj)
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

        public BannerTran Find(int id)
        {
            return _entities.BannerTrans.Find(id);
        }
        
        public IEnumerable<BannerTran> GetAll()
        {
            return _entities.BannerTrans;
        }

        public IEnumerable<BannerModel> GetByBannerID(int bannerID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT b.ID, b.Image, bt.Title, bt. Description, bt. Contents, bt.BannerID, l.LangName FROM BannerTrans bt ");
                    sb.Append("JOIN Banner b ON b.ID = bt.BannerID ");
                    sb.Append("JOIN tbl_Languages l ON bt.LangCode = l.LangCode ");
                    sb.Append("WHERE bt.BannerID = @BannerID");

                    var parameters = new DynamicParameters();
                    parameters.Add("BannerID", bannerID);
                    return conn.Query<BannerModel>(
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
    }
}
