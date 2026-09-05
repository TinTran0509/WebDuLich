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
    public class IntroductionTransRepository : IIntroductionTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Introduction";

        public void Add(IntroductionTran model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.IntroductionTrans.Add(model);
            _entities.SaveChanges();
        } 

        public void Edit(IntroductionTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE IntroductionTrans ");
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

        public IntroductionTran Find(int id)
        {
            return _entities.IntroductionTrans.Find(id);
        }
        
        public IEnumerable<IntroductionTran> GetAll()
        {
            return _entities.IntroductionTrans;
        }

        public IEnumerable<IntroductionViewModel> GetByIntroductionID(int introductionID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT b.ID, b.Image, bt.Title, bt. Description, bt. Contents, bt.ProductID, l.LangName FROM IntroductionTrans bt ");
                    sb.Append("JOIN Introduction b ON b.ID = bt.IntroductionID ");
                    sb.Append("JOIN tbl_Languages l ON bt.LangCode = l.LangCode ");
                    sb.Append("WHERE bt.IntroductionID = @IntroductionID");

                    var parameters = new DynamicParameters();
                    parameters.Add("IntroductionID", introductionID);
                    return conn.Query<IntroductionViewModel>(
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

        public IntroductionViewModel GetByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT i.Image, it.Title, it. Description,it. Contents FROM IntroductionTrans it ");
                    sb.Append("JOIN Introduction i ON i.ID = it.IntroductionID "); 
                    sb.Append("WHERE it.LangCode = @LangCode");

                    var parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);
                    return conn.Query<IntroductionViewModel>(
                        sb.ToString(),
                        parameters,
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
