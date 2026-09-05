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
using Web.Model.CustomModel;

namespace Web.Repository.Entity
{
    public class ProductTransRepository : IProductTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "ProductTrans";

        public void Add(ProductTran model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.ProductTrans.Add(model);
            _entities.SaveChanges();
        } 

        public void Edit(ProductTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE BannerTrans ");
                    sb.Append("SET Title = @Title,LinkSeo = @LinkSeo, ");
                    sb.Append("Description = @Description,Contents = @Contents ");
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Title", obj.Title);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("Description", obj.Description);
                    parameters.Add("Contents", obj.Contents); 
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
        
        public IEnumerable<ProductTran> GetAll()
        {
            return _entities.ProductTrans;
        }

        public IEnumerable<ProductModel> GetByProductID(int productID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT b.ID, b.Image, bt.Title, bt. Description, bt. Contents, bt.ProductID, l.LangName FROM ProductTrans bt ");
                    sb.Append("JOIN Product b ON b.ID = bt.ProductID ");
                    sb.Append("JOIN tbl_Languages l ON bt.LangCode = l.LangCode ");
                    sb.Append("WHERE bt.ProductID = @ProductID");

                    var parameters = new DynamicParameters();
                    parameters.Add("ProductID", productID);
                    return conn.Query<ProductModel>(
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

        public IEnumerable<ProductModel> GetByType(int type, string langCode, int takeRow)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT p.ID, p.Image, pt.Title, pt. Description, pt. Contents, pt.ProductID, l.LangName FROM ProductTrans pt ");
                    sb.Append("JOIN Product p ON p.ID = pt.ProductID ");
                    sb.Append("JOIN tbl_Languages l ON pt.LangCode = l.LangCode ");
                    sb.Append("WHERE p.Type = @Type AND pt.LangCode = @LangCode ");
                    sb.Append("ORDER BY p.CreatedDate DESC ");
                    sb.Append($"OFFSET 0 ROWS FETCH NEXT {takeRow} ROWS ONLY");

                    var parameters = new DynamicParameters();
                    parameters.Add("Type", type);
                    parameters.Add("LangCode", langCode); 

                    return conn.Query<ProductModel>(
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
