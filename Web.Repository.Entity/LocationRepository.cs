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
    public class LocationRepository : ILocationRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Location";

        public void Add(LocationTran obj)
        {
            _entities.LocationTrans.Add(obj);
            _entities.SaveChanges();
        }

        public int Create(Location model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters(); 
                    parameters.Add("Image", model.Image);
                    parameters.Add("Active", model.Active);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Location_Insert",
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
            try
            {
                using (var connection = new SqlConnection(_connectString))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        string sqlMenu = $"DELETE FROM Location WHERE ID = @ID";
                        DynamicParameters parameters1 = new DynamicParameters();
                        parameters1.Add("ID", id);
                        connection.Execute(
                            sqlMenu,
                            parameters1,
                            commandType: CommandType.Text,
                            transaction: tran);

                        string sql = $"DELETE LocationTrans WHERE LocationID = @LocationID";
                        DynamicParameters parameters2 = new DynamicParameters();
                        parameters2.Add("LocationID", id);
                        connection.Execute(
                            sql,
                             parameters2,
                           commandType: CommandType.Text,
                           transaction: tran);

                        tran.Commit();
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            { 
                throw;
            } 
        }

        public void Edit(Location obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(LocationTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE LocationTrans ");
                    sb.Append("SET Name = @Name,Description = @Description "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Name", obj.Name);
                    parameters.Add("Description", obj.Description);
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

        public Location Find(int id)
        {
            return _entities.Locations.Find(id);
        }

        public IEnumerable<LocationViewModel> GetAll()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT lc.*,lt.Name,lt.Description,lt.LocationID,l.LangName FROM Location lc ");
                    sb.Append("JOIN LocationTrans lt ON lt.LocationID = lc.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = lt.LangCode");

                    return conn.Query<LocationViewModel>(
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

        public IEnumerable<LocationTran> GetByLocationID(string locationIDs, string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                { 
                    string sql = $"SELECT * FROM LocationTrans WHERE LocationID IN ({locationIDs}) AND LangCode = @LangCode";
                   
                    DynamicParameters parameters = new DynamicParameters(); 
                    parameters.Add("LangCode", langCode);

                    return conn.Query<LocationTran>(
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

        public IEnumerable<LocationTran> GetAllLocationTrans()
        {
            return _entities.LocationTrans;
        } 
    }
}
