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
                    parameters.Add("ImageItinerary", obj.ImageItinerary);
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
                    parameters.Add("Itineraries", obj.Itineraries);
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
                    parameters.Add("ImageItinerary", obj.ImageItinerary);
                    parameters.Add("MenuID", obj.MenuID);
                    parameters.Add("Active", obj.Active);
                    parameters.Add("Type", obj.Type);
                    parameters.Add("Price", obj.Price);
                    parameters.Add("PriceWeekend", obj.PriceWeekend);
                    parameters.Add("Size", obj.Size);
                    parameters.Add("DayNumber", obj.DayNumber);
                    parameters.Add("CountryID", obj.CountryID);
                    parameters.Add("LocationID", obj.LocationID);
                    parameters.Add("HotelID", obj.HotelID);
                    parameters.Add("NumberStar", obj.NumberStar);
                    parameters.Add("Itineraries", obj.Itineraries);
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

        public void Update(Product obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE Product ");
                    sb.Append("SET ImageItinerary = @ImageItinerary,Itineraries = @Itineraries "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("ImageItinerary", obj.ImageItinerary);
                    parameters.Add("Itineraries", obj.Itineraries); 
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

        public void InsertItinerary(int prodId, List<Itinerary> Itineraries)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    string sqlItinerary = $"DELETE FROM Itinerary WHERE ProductID = @ProductID";
                    DynamicParameters parameters1 = new DynamicParameters();
                    parameters1.Add("ProductID", prodId);
                    connection.Execute(
                        sqlItinerary,
                        parameters1,
                        commandType: CommandType.Text,
                        transaction: tran); 

                    foreach (var item in Itineraries)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO Itinerary (Title ,Contents ,LangCode ,ProductID) ");
                        sb.Append("VALUES (@Title ,@Contents  ,@LangCode  ,@ProductID) "); 

                        DynamicParameters parameters2 = new DynamicParameters();
                        parameters2.Add("Title", item.Title);
                        parameters2.Add("Contents", item.Contents);
                        parameters2.Add("LangCode", item.LangCode);
                        parameters2.Add("ProductID", item.ProductID);
                        connection.Execute(sb.ToString(),
                            parameters2,
                            commandType: CommandType.Text,
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
                    string sqlPackage = $"DELETE FROM Package_Price WHERE ProductID = @ProductID";
                    DynamicParameters parameters1 = new DynamicParameters();
                    parameters1.Add("ProductID", id);
                    connection.Execute(
                        sqlPackage,
                        parameters1,
                        commandType: CommandType.Text,
                        transaction: tran);

                    string sqlProd = $"DELETE FROM Product WHERE ID = @ID";
                    DynamicParameters parameters2 = new DynamicParameters();
                    parameters2.Add("ID", id);
                    connection.Execute(
                        sqlProd,
                        parameters2,
                        commandType: CommandType.Text,
                        transaction: tran);

                    string sqlItinerary = $"DELETE FROM Itinerary WHERE ProductID = @ProductID";
                    DynamicParameters parameters3 = new DynamicParameters();
                    parameters3.Add("ProductID", id);
                    connection.Execute(
                        sqlItinerary,
                        parameters3,
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
    }
}
