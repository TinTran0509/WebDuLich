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
    public class BannerRepository : IBannerRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Banner";

        public int Add(Banner obj)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Image", obj.Image);
                    parameters.Add("MenuID", obj.MenuID);
                    parameters.Add("Active", obj.Active);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Banners_Insert",
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

        public void Create(BannerModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(_connectString))
                {
                    using (var conn = new SqlConnection(_connectString))
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Image", obj.Image);
                        parameters.Add("Active", true);
                        parameters.Add("Title", obj.Title);
                        parameters.Add("LinkSeo", obj.LinkSeo);
                        parameters.Add("Description", obj.Description);
                        parameters.Add("Contents", obj.Contents);
                        parameters.Add("LangCode", obj.LangCode); 
                        parameters.Add("MenuID", obj.MenuID); 
                        conn.Execute("Sp_Banner_Insert",
                            parameters,
                            commandType: CommandType.StoredProcedure); 
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            { 
                throw;
            } 
        }

        public void Delete(int id, List<BannerTran> bannerTrans)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    string sqlMenu = $"DELETE FROM Banner WHERE ID = @ID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", id);
                    connection.Execute(
                        sqlMenu,
                        parameters,
                        commandType: CommandType.Text,
                        transaction: tran);

                    foreach (var item in bannerTrans)
                    {
                        string sql = $"DELETE BannerTrans WHERE ID ={item.ID}";
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

        public void Edit(Banner obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public Banner Find(int id)
        {
            return _entities.Banners.Find(id);
        }
        
        public IEnumerable<Banner> GetAll()
        {
            return _entities.Banners;
        }

        public IEnumerable<BannerModel> GetByPage(int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<BannerModel> lst = conn.Query<BannerModel>(
                            "Sp_Theme_GetPage",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                    var pageAdminMenu = lst.ToList();
                    total = pageAdminMenu.Any() ? pageAdminMenu.FirstOrDefault().TotalCount : 0;
                    return pageAdminMenu;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
