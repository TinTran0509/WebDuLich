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
    public class UserRepository : IUserRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        public void Add(tbl_User obj)
        {
            _entities.tbl_User.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.tbl_User.Remove(obj);
            _entities.SaveChanges();
        }
        public void Edit(tbl_User obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }
        public void Edit(List<tbl_User> lstobj)
        {
            foreach (var obj in lstobj)
            {
                _entities.Entry(obj).State = EntityState.Modified;
            }
            _entities.SaveChanges();
        }
        public tbl_User Find(int id)
        {
            return _entities.tbl_User.Find(id);
        }

        public IEnumerable<tbl_User> GetAll()
        {
            return _entities.tbl_User;
        }

        public UserModel GetByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT u.*,ut.Description FROM tbl_User u ");
                    sb.Append("JOIN UserTrans ut ON ut.UserID = u.ID ");
                    sb.Append("WHERE ut.LangCode = @LangCode");

                    var parameters = new DynamicParameters();
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

        public UserModel GetByLangID(int langID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = "SELECT * FROM tbl_User WHERE LangID = @LangID";

                    var parameters = new DynamicParameters();
                    parameters.Add("LangID", langID); 

                    return conn.Query<UserModel>(
                        sql,
                        parameters,
                        commandType: CommandType.Text).FirstOrDefault();
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

        public IEnumerable<UserModel> GetAllByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT u.FullName, u. Phone, u.Photo, ut.Description, l.Icon  AS Flag, l.LangName FROM tbl_User u ");
                    sb.Append("JOIN UserTrans ut ON ut.UserID = u.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.ID = u.LangID ");
                    sb.Append("WHERE ut.LangCode = @LangCode");

                    var parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);

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
    }
}
