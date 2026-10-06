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
                    sb.Append("SELECT p.ID, p.Image, p.ImageItinerary,p.ProductCode, pt.Title, pt. Description, pt. Contents, pt.ProductID, l.LangName FROM ProductTrans pt ");
                    sb.Append("JOIN Product p ON p.ID = pt.ProductID ");
                    sb.Append("JOIN tbl_Languages l ON pt.LangCode = l.LangCode ");
                    sb.Append("WHERE pt.ProductID = @ProductID");

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

        public IEnumerable<ProductModel> GetByType(int type, string langCode, int productId, int takeRow)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"SELECT TOP {takeRow} p.ID, p.Image, p.DayNumber, p.Price, p.NumberStar, pt.Title, pt. Description,pt.LangCode, pt.LinkSeo,");
                    sb.Append("(SELECT STUFF((SELECT ' - ' + ct.Name FROM CountryTrans ct WHERE ct.CountryID IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(p.CountryID, ',')) AND ct.LangCode = pt.LangCode FOR XML PATH('')), 1, 2, '')) AS Countries,");
                    sb.Append("(SELECT STUFF((SELECT ', ' + lt.Name FROM LocationTrans lt WHERE lt.LocationID IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(p.LocationID, ',')) AND lt.LangCode ='EN' FOR XML PATH('')), 1, 2, '')) AS Locations,");
                    sb.Append("pt. Contents, pt.ProductID, l.LangName FROM ProductTrans pt ");
                    sb.Append("JOIN Product p ON p.ID = pt.ProductID ");
                    sb.Append("JOIN tbl_Languages l ON pt.LangCode = l.LangCode ");
                    sb.Append("WHERE p.Type = @Type AND pt.LangCode = @LangCode ");
                    if (productId != 0)
                    {
                        sb.Append($"AND p.ID = {productId} ");
                    } 
                    sb.Append("ORDER BY p.CreatedDate DESC "); 

                    var parameters = new DynamicParameters();
                    parameters.Add("Type", type);
                    parameters.Add("LangCode", langCode); 

                    string sql = sb.ToString(); 

                    return conn.Query<ProductModel>(
                        sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
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
                    sb.Append("SELECT p.ProductCode,p.MenuID,p.Image,p.ImageItinerary,p.Type,p.Price,p.PriceWeekend,p.Size,p.DayNumber,p.NumberStar,p.LocationID,pt.LangCode,lg.LangName,");
                    sb.Append("lg.FullCode AS Culture, p.LocationID,pt.Title, pt.Contents,pt.Description,pt.LinkSeo,pt.ID, pt.ProductID FROM Product p ");
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

        public ProductModel GetByProductTransId(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT p.ProductCode,p.MenuID,p.Image,p.Type,p.Price,p.PriceWeekend,p.Size,p.DayNumber,p.NumberStar,p.LocationID,pt.LangCode,lg.LangName,");
                    sb.Append("lg.FullCode AS Culture,p.LocationID,pt.Title, pt.Contents,pt.Description,pt.LinkSeo,pt.ProductID FROM Product p ");
                    sb.Append("JOIN ProductTrans pt ON pt.ProductID = p.ID ");
                    sb.Append("JOIN tbl_Languages lg ON lg.LangCode = pt.LangCode ");
                    sb.Append("WHERE pt.ID = @ID");

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

        public IEnumerable<Itinerary> GetItineraryByProductID(int productID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = "SELECT * FROM Itinerary WHERE ProductID = @ProductID";

                    var parameters = new DynamicParameters();
                    parameters.Add("ProductID", productID); 
                    return conn.Query<Itinerary>(
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

        public IEnumerable<Itinerary> GetItineraryByProductIDAndLangCode(int productID, string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    string sql = "SELECT * FROM Itinerary WHERE ProductID = @ProductID AND LangCode = @LangCode";
                     
                    var parameters = new DynamicParameters();
                    parameters.Add("ProductID", productID);
                    parameters.Add("LangCode", langCode);
                    return conn.Query<Itinerary>(
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

        public IEnumerable<ProductModel> GetRelate(int prodId, int menuId, string langCode, int takeRow)
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
                    sb.Append("WHERE p.MenuID = @MenuID AND pt.LangCode = @LangCode AND pt.ProductID != @ProductID ");
                    sb.Append("ORDER BY p.CreatedDate DESC ");

                    var parameters = new DynamicParameters();
                    parameters.Add("MenuID", menuId);
                    parameters.Add("LangCode", langCode);
                    parameters.Add("ProductID", prodId);

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

        public IEnumerable<ProductModel> GetByPage(string code, string title, int type, int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Code", code);
                    parameters.Add("Title", title);
                    parameters.Add("Type", type);
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<ProductModel> lst = conn.Query<ProductModel>(
                            "Sp_Product_GetPage",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                    var pageBooking = lst.ToList();
                    total = pageBooking.Any() ? pageBooking.FirstOrDefault().TotalCount : 0;
                    return pageBooking;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
