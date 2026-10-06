using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Web.Model;
using Web.Model.CustomModel;
using Web.Model.Domain;

namespace Web.Repository.Entity
{
    public class NewsRepository : INewsRepository
    {
        readonly WebDuLichEntities context = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        public int Add(News model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("CategoryId", model.CategoryId);
                    parameters.Add("Image", model.Image);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_News_Insert",
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

        public void AddNewsTrans(NewsTran model)
        {
             context.NewsTrans.Add(model);
            context.SaveChanges();
        }

        public IEnumerable<ListNews> ListAll(string keyWord, int status)
        {
            object[] parameters =
            {
                new SqlParameter("@MetaTitle",keyWord),
                new SqlParameter("@Status",status),
            };
            return context.Database.SqlQuery<ListNews>("Sp_News_ListAll @MetaTitle,@Status", parameters);
        }

        public IEnumerable<News> GetAll()
        {
            return context.News;
        }

        public IEnumerable<NewsTran> GetAllNewsTrans()
        {
            return context.NewsTrans;
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            context.News.Remove(obj);
            context.SaveChanges();
        }

        public void Edit(News obj, List<NewsTran> newsTrans)
        {
            using (var connection = new SqlConnection(_connectString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("CategoryId", obj.CategoryId);
                    parameters.Add("Image", obj.Image);    
                    parameters.Add("ModifiedBy", obj.ModifiedBy); 
                    connection.Execute("Sp_News_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        transaction: tran);

                    foreach (var item in newsTrans)
                    {
                        DynamicParameters parametersTrans = new DynamicParameters();
                        parametersTrans.Add("ID", item.ID);
                        parametersTrans.Add("MetaTitle", item.MetaTitle);
                        parametersTrans.Add("LinkSeo", item.LinkSeo);
                        parametersTrans.Add("Description", item.Description);
                        parametersTrans.Add("Contents", item.Contents);
                        connection.Execute("Sp_NewsTrans_Update",
                           parametersTrans,
                           commandType: CommandType.StoredProcedure,
                           transaction: tran);
                    }

                    tran.Commit();
                }
                connection.Close();
            }
        }

        public News FindByTitle(string title)
        {
            return context.Database.SqlQuery<News>("Sp_News_GetByTitle @MetaTitle", new SqlParameter("@MetaTitle", title)).FirstOrDefault();
        } 

        public News FindByLinkSeo(string linkseo)
        {
            return context.Database.SqlQuery<News>("Sp_News_GetByLinkSeo @LinkSeo", new SqlParameter("@LinkSeo", linkseo)).FirstOrDefault();
        }

        public News Find(int id)
        {
            return context.News.Find(id);
        }

        public IEnumerable<NewsModel> GetByPage(string title, int cateId, int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    var parameters = new DynamicParameters(); 
                    parameters.Add("Title", title);
                    parameters.Add("CategoryID", cateId);
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<NewsModel> lst = conn.Query<NewsModel>(
                            "Sp_News_GetPage",
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
