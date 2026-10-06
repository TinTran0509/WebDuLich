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

namespace Web.Repository.Entity
{
    public class PackageRepository : IPackageRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "PackageImages"; 

        public void AddPackagePrice(Package_Price obj)
        {
            _entities.Package_Price.Add(obj);
            _entities.SaveChanges();
        }

        public void UpdatePackagePrice(Package_Price obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE Package_Price ");
                    sb.Append("SET Price = @Price ");
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Price", obj.Price); 
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

        public IEnumerable<Package_Price> GetAllPackagePrice()
        {
            return _entities.Package_Price;
        }

        public void TrunCatePackage()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = "TRUNCATE TABLE Package_Price";
 
                    conn.Execute(sql,
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
