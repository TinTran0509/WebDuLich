using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository.Entity
{
    public class IntroductionRepository : IIntroductionRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Introduction";

        public int Add(Introduction model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Image", model.Image);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Introduction_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    id = parameters.Get<int>("@ID");
                }
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Edit(Introduction obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE Introduction ");
                    sb.Append("SET Image = @Image "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Image", obj.Image); 
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

        public Introduction Find(int id)
        {
            return _entities.Introductions.Find(id);
        }

        public IEnumerable<IntroductionViewModel> GetAll()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT i.ID, i.Image, it.Title, it. Description,it. Contents,it.IntroductionID, l.LangName FROM IntroductionTrans it ");
                    sb.Append("JOIN Introduction i ON i.ID = it.IntroductionID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = it.LangCode");

                    return conn.Query<IntroductionViewModel>(
                        sb.ToString(),
                        null,
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
