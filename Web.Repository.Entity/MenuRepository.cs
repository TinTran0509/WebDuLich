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
    public class MenuRepository : IMenuRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Menu";
        public int Add(Menu obj)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters(); 
                    parameters.Add("Image", obj.Image); 
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Menu_Insert",
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
        public void Delete(int id, List<MenuTran> menuTrans)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    string sqlMenu = $"DELETE FROM Menu WHERE ID = @ID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", id);
                    connection.Execute(
                        sqlMenu,
                        parameters,
                        commandType: CommandType.Text,
                        transaction: tran);

                    foreach (var item in menuTrans)
                    {
                        string sql = $"DELETE MenuTrans WHERE ID ={item.ID}";
                        connection.Execute(
                            sql,
                           commandType: CommandType.Text,
                           transaction: tran);
                    } 

                    tran.Commit();
                } 
                connection.Close();
            }
        }
       
        public Menu Find(int id)
        {
            return _entities.Menus.Find(id);
        }
         
        public IEnumerable<Menu> GetAll()
        {
            return _entities.Menus;
        }

        public IEnumerable<MenuTranModel> GetByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT mt.* FROM Menu m ");
                    sb.Append("LEFT JOIN MenuTrans mt ON m.ID = mt.MenuID ");
                    sb.Append("WHERE mt.LangCode = @LangCode ORDER BY mt.Ordering");

                    var parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);
                    return conn.Query<MenuTranModel>(
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
