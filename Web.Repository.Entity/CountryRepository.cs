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
                    parameters.Add("Flag", model.Flag);
                    parameters.Add("Active", model.Active);
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
                    sb.Append("UPDATE CountryTran ");
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
            catch (Exception)
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
                    sb.Append("SELECT c.*,ct.Name,l.LangName FROM Country c ");
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

        public IEnumerable<CountryTran> GetAllCountryTrans()
        {
            return _entities.CountryTrans;
        }

        public IEnumerable<CountryUser> GetByHeader()
        {
            return _entities.Database.SqlQuery<CountryUser>("Sp_Countries_Users");
        }
    }
}
