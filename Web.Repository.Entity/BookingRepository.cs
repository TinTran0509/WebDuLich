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
    public class BookingRepository : IBookingRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"]; 

        public void Add(Booking obj)
        {
            _entities.Bookings.Add(obj);
            _entities.SaveChanges();
        }

        public void Create(Booking obj, SentMail mail)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("ArrivalDate", obj.ArrivalDate);
                        parameters.Add("TotalPax", obj.TotalPax);
                        parameters.Add("EstimatedTotal", obj.EstimatedTotal);
                        parameters.Add("Title", obj.Title);
                        parameters.Add("FirstName", obj.FirstName);
                        parameters.Add("LastName", obj.LastName);
                        parameters.Add("Tel", obj.Tel);
                        parameters.Add("Email", obj.Email);
                        parameters.Add("Language", obj.Language);
                        parameters.Add("Notes", obj.Notes);
                        parameters.Add("Status", obj.Status);
                        parameters.Add("CreatedDate", obj.CreatedDate);
                        parameters.Add("ProductID", obj.ProductID);
                        parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute("Sp_Booking_Insert",
                            parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        int bookingId = parameters.Get<int>("@ID");

                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO SentMail (Status ,CreatedDate ,Subject ,BookingID) ");
                        sb.Append("VALUES (@Status ,@CreatedDate  ,@Subject  ,@BookingID) ");

                        DynamicParameters parameters2 = new DynamicParameters();
                        parameters2.Add("Status", mail.Status);
                        parameters2.Add("CreatedDate", mail.CreatedDate);
                        parameters2.Add("Subject", mail.Subject);
                        parameters2.Add("BookingID", bookingId);
                        connection.Execute(sb.ToString(),
                            parameters2,
                            commandType: CommandType.Text,
                            transaction: tran);
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                    tran.Commit();
                }
                connection.Close();
            }
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Bookings.Remove(obj);
            _entities.SaveChanges();
        } 

        public Booking Find(int id)
        {
            return _entities.Bookings.Find(id);
        }

        public IEnumerable<BookingModel> GetByPage(string keySearh, int status, int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Keyword", keySearh);
                    parameters.Add("Status", status);
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<BookingModel> lst = conn.Query<BookingModel>(
                            "Sp_Booking_GetPage",
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
