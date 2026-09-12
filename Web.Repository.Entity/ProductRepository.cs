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
    public class ProductRepository : IProductRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Product";

        public int Add(Product obj)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ProductCode", obj.ProductCode);
                    parameters.Add("Image", obj.Image);
                    parameters.Add("MenuID", obj.MenuID);
                    parameters.Add("Active", obj.Active);
                    parameters.Add("Type", obj.Type);
                    parameters.Add("Price", obj.Price);
                    parameters.Add("Size", obj.Size);
                    parameters.Add("DayNumber", obj.DayNumber);
                    parameters.Add("CountryID", obj.CountryID);
                    parameters.Add("LocationID", obj.LocationID);
                    parameters.Add("HotelID", obj.HotelID);
                    parameters.Add("NumberStar", obj.NumberStar);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Product_Insert",
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

        public void Edit(Product obj, List<ProductTran> productTrans)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("ProductCode", obj.ProductCode);
                    parameters.Add("Image", obj.Image);
                    parameters.Add("MenuID", obj.MenuID);
                    parameters.Add("Active", obj.Active);
                    parameters.Add("Type", obj.Type);
                    parameters.Add("Price", obj.Price);
                    parameters.Add("Size", obj.Size);
                    parameters.Add("DayNumber", obj.DayNumber);
                    parameters.Add("CountryID", obj.CountryID);
                    parameters.Add("LocationID", obj.LocationID);
                    parameters.Add("HotelID", obj.HotelID);
                    parameters.Add("NumberStar", obj.NumberStar);
                    connection.Execute("Sp_Product_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        transaction: tran);

                    foreach (var item in productTrans)
                    {
                        DynamicParameters parametersTrans = new DynamicParameters();
                        parametersTrans.Add("ID", item.ID);
                        parametersTrans.Add("Title", item.Title);
                        parametersTrans.Add("LinkSeo", item.LinkSeo);
                        parametersTrans.Add("Description", item.Description);
                        parametersTrans.Add("Contents", item.Contents);
                        connection.Execute("Sp_ProductTrans_Update",
                           parametersTrans,
                           commandType: CommandType.StoredProcedure,
                           transaction: tran);
                    }

                    tran.Commit();
                }
                connection.Close();
            }
        }

        public void Delete(int id, List<ProductTran> productTrans)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    string sqlMenu = $"DELETE FROM Product WHERE ID = @ID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", id);
                    connection.Execute(
                        sqlMenu,
                        parameters,
                        commandType: CommandType.Text,
                        transaction: tran);

                    foreach (var item in productTrans)
                    {
                        string sql = $"DELETE ProductTrans WHERE ID ={item.ID}";
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

        public Product Find(int id)
        {
            return _entities.Products.Find(id);
        }
        
        public IEnumerable<Product> GetAll()
        {
            return _entities.Products;
        }

        public IEnumerable<ProductModel> GetByPage(int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<ProductModel> lst = conn.Query<ProductModel>(
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

        public ProductModel GetByLinkSeo(string linkSeo)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT p.ProductCode,p.MenuID,p.Image,p.Type,p.Price,p.Size,p.DayNumber,p.NumberStar,p.LocationID,pt.LangCode,lg.LangName,");
                    sb.Append("p.LocationID,pt.Title, pt.Contents,pt.Description,pt.LinkSeo,pt.ProductID FROM Product p ");
                    sb.Append("JOIN ProductTrans pt ON pt.ProductID = p.ID ");
                    sb.Append("JOIN tbl_Languages lg ON lg.LangCode = pt.LangCode ");
                    sb.Append("WHERE pt.LinkSeo = @LinkSeo");

                    var parameters = new DynamicParameters();
                    parameters.Add("LinkSeo", linkSeo);
                    return conn.Query<ProductModel>(
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

        public ProductModel GetById(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT p.ProductCode,p.MenuID,p.Image,p.Type,p.Price,p.Size,p.DayNumber,p.NumberStar,p.LocationID,pt.LangCode,lg.LangName,");
                    sb.Append("p.LocationID,pt.Title, pt.Contents,pt.Description,pt.LinkSeo,pt.ProductID FROM Product p ");
                    sb.Append("JOIN ProductTrans pt ON pt.ProductID = p.ID ");
                    sb.Append("JOIN tbl_Languages lg ON lg.LangCode = pt.LangCode ");
                    sb.Append("WHERE p.ID = @ID");

                    var parameters = new DynamicParameters();
                    parameters.Add("ID", id);
                    return conn.Query<ProductModel>(
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

        public IEnumerable<ProductModel> GetRelate(int menuId, string langCode, int takeRow)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"SELECT TOP {takeRow} p.ID, p.Image, p.DayNumber, p.Price, p.NumberStar, pt.Title, pt. Description, pt.LinkSeo,");
                    sb.Append("(SELECT STUFF((SELECT ' - ' + ct.Name FROM CountryTrans ct WHERE ct.CountryID IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(p.CountryID, ',')) AND ct.LangCode = pt.LangCode FOR XML PATH('')), 1, 2, '')) AS Countries,");
                    sb.Append("(SELECT STUFF((SELECT ', ' + lt.Name FROM LocationTrans lt WHERE lt.LocationID IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(p.LocationID, ',')) AND lt.LangCode ='EN' FOR XML PATH('')), 1, 2, '')) AS Locations,");
                    sb.Append("pt. Contents, pt.ProductID, l.LangName FROM ProductTrans pt ");
                    sb.Append("JOIN Product p ON p.ID = pt.ProductID ");
                    sb.Append("JOIN tbl_Languages l ON pt.LangCode = l.LangCode ");
                    sb.Append("WHERE p.MenuID = @MenuID AND pt.LangCode = @LangCode ");
                    sb.Append("ORDER BY p.CreatedDate DESC ");

                    var parameters = new DynamicParameters();
                    parameters.Add("MenuID", menuId);
                    parameters.Add("LangCode", langCode);

                    string sql = sb.ToString();

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
