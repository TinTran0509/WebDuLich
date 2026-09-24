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
    public class CategoryRepository : ICategoryRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "cachecategory";
        public int Add(Category model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Ordering", model.Ordering); 
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Category_Insert",
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

        public void AddTrans(CategoryTran model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.CategoryTrans.Add(model);
            _entities.SaveChanges();
        } 

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Categories.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Category model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectString))
                {
                    using (var conn = new SqlConnection(_connectString))
                    {
                        string sql = "UPDATE Category SET Ordering = @Ordering WHERE ID = @ID";
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Ordering", model.Ordering); 
                        parameters.Add("ID", model.ID);
                        conn.Execute(sql,
                            parameters,
                            commandType: CommandType.Text);
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EditTrans(CategoryTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE CategoryTrans ");
                    sb.Append("SET Name = @Name,LinkSeo = @LinkSeo "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Name", obj.Name);
                    parameters.Add("LinkSeo", obj.LinkSeo); 
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

        public Category Find(int id)
        {
            return _entities.Categories.Find(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _entities.Categories;
        }

        public IEnumerable<CategoryTran> GetAllTrans()
        {
            return _entities.CategoryTrans;
        }

        public IEnumerable<CategoryModel> GetCategoryTranByCategoryID(int categoryID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT c.ID, c.Ordering, l.LangName, ct.Name,ct.CategoryID  FROM Category c ");
                    sb.Append("JOIN CategoryTrans ct ON ct.CategoryID = c.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = ct.LangCode ");
                    sb.Append("WHERE c.ID = @ID");

                    var parameters = new DynamicParameters();
                    parameters.Add("ID", categoryID);
                    return conn.Query<CategoryModel>(
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
