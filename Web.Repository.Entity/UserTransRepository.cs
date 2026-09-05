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
using Web.Model;
using Web.Model.CustomModel;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.Repository.Entity
{
    public class UserTransRepository : IUserTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        public void Add(UserTran obj)
        {
            _entities.UserTrans.Add(obj);
            _entities.SaveChanges();
        }

        public int Create(UserTran obj)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", obj.LangCode);
                    parameters.Add("Address", obj.Address);
                    parameters.Add("Description", obj.Description);
                    parameters.Add("UserID", obj.UserID);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_UserTrans_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    id = parameters.Get<int>("@ID");
                }
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.UserTrans.Remove(obj);
            _entities.SaveChanges();
        }
        public void Edit(UserTran obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }
        public void Edit(List<UserTran> lstobj)
        {
            foreach (var obj in lstobj)
            {
                _entities.Entry(obj).State = EntityState.Modified;
            }
            _entities.SaveChanges();
        }
        public UserTran Find(int id)
        {
            return _entities.UserTrans.Find(id);
        }

        public IEnumerable<UserTran> GetAll()
        {
            return _entities.UserTrans;
        }

        public IEnumerable<UserModel> GetByUserID(int userID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT ut.*,l.LangName AS Language FROM UserTrans ut ");
                    sb.Append("JOIN tbl_Languages l ON ut.LangCode = l.LangCode ");
                    sb.Append("WHERE ut.UserID = @UserID ");
                    sb.Append("ORDER BY ut.ID");

                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userID);
                    
                    return conn.Query<UserModel>(
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

        public UserModel GetByCountry(int countryID, string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT u.*, ut.Address, ut.Description FROM tbl_User u ");
                    sb.Append("LEFT JOIN UserTrans ut ON ut.UserID = u.ID ");
                    sb.Append("WHERE u.CountryID = @CountryID AND ut.LangCode = @LangCode");

                    var parameters = new DynamicParameters();
                    parameters.Add("CountryID", countryID);
                    parameters.Add("LangCode", langCode);

                    return conn.Query<UserModel>(
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
