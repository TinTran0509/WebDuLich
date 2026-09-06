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
    public class HotelRepository : IHotelRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Hotel";

        public void Add(HotelTran obj)
        {
            _entities.HotelTrans.Add(obj);
            _entities.SaveChanges();
        }

        public int Create(Hotel model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters(); 
                    parameters.Add("Image", model.Image);
                    parameters.Add("Active", model.Active);
                    parameters.Add("Rating", model.Rating);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Hotel_Insert",
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
                        string sqlMenu = $"DELETE FROM Hotel WHERE ID = @ID";
                        DynamicParameters parameters1 = new DynamicParameters();
                        parameters1.Add("ID", id);
                        connection.Execute(
                            sqlMenu,
                            parameters1,
                            commandType: CommandType.Text,
                            transaction: tran);

                        string sql = $"DELETE HotelTrans WHERE HotelID = @HotelID";
                        DynamicParameters parameters2 = new DynamicParameters();
                        parameters2.Add("HotelID", id);
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

        public void Edit(Hotel obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(HotelTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE HotelTrans ");
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

        public Hotel Find(int id)
        {
            return _entities.Hotels.Find(id);
        }

        public IEnumerable<HotelViewModel> GetAll()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT lc.*,lt.Name,lt.Description,lt.HotelID,l.LangName FROM Hotel lc ");
                    sb.Append("JOIN HotelTrans lt ON lt.HotelID = lc.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = lt.LangCode");

                    return conn.Query<HotelViewModel>(
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

        public IEnumerable<HotelTran> GetAllHotelTrans()
        {
            return _entities.HotelTrans;
        } 
    }
}
