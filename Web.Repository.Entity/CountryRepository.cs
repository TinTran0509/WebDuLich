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
    public class CountryRepository : ICountryRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "cachecategory";

        public void Add(CountryTran obj)
        {
            _entities.CountryTrans.Add(obj);
            _entities.SaveChanges();
        }

        public int Create(Country model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Code", model.Code);
                    parameters.Add("Image", model.Image);
                    parameters.Add("Active", model.Active);
                    parameters.Add("Ordering", model.Ordering);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Country_Insert",
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

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Countries.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Country obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(CountryTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE CountryTrans ");
                    sb.Append("SET Name = @Name "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Name", obj.Name); 
                    conn.Execute(sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Country Find(int id)
        {
            return _entities.Countries.Find(id);
        }

        public IEnumerable<CountryViewModel> GetAll()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT c.*,ct.Name,ct.CountryID,l.LangName FROM Country c ");
                    sb.Append("JOIN CountryTrans ct ON ct.CountryID = c.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = ct.LangCode");

                    return conn.Query<CountryViewModel>(
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

        public IEnumerable<CountryTran> GetCountryTranByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                { 
                    string sql = "SELECT ct.* FROM CountryTrans ct WHERE ct.LangCode = @LangCode";
                  
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode); 

                    return conn.Query<CountryTran>(
                        sql,
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<CountryViewModel> GetCountryViewModelByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT c.Image, ct.* FROM CountryTrans ct ");
                    sb.Append("JOIN Country c ON c.ID = ct.CountryID ");
                    sb.Append("WHERE ct.LangCode = @LangCode"); 

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);

                    return conn.Query<CountryViewModel>(
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

        public IEnumerable<CountryTran> GetCountryTranByCountryID(int countryID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = "SELECT ct.* FROM CountryTrans ct WHERE ct.CountryID = @CountryID";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("CountryID", countryID);

                    return conn.Query<CountryTran>(
                        sql,
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<CountryTran> GetAllCountryTrans()
        {
            return _entities.CountryTrans;
        }

        public IEnumerable<CountryUser> GetByHeader()
        {
            return _entities.Database.SqlQuery<CountryUser>("Sp_Countries_Users");
        }

        public IEnumerable<CountryTran> GetByCountryID(string countryIDs, string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = $"SELECT * FROM CountryTrans WHERE CountryID IN ({countryIDs}) AND LangCode = @LangCode";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);

                    return conn.Query<CountryTran>(
                        sql,
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
