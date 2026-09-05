using System;
using System.Collections.Generic;
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
       
        public void Add(News model)
        {
            object[] parameters =
            {
                new SqlParameter("@CategoryId", model.CategoryId),
                new SqlParameter("@MetaTitle", (object)model.MetaTitle??DBNull.Value),
                new SqlParameter("@LinkSeo", model.LinkSeo),
                new SqlParameter("@Image",(object)model.Image??DBNull.Value),
                new SqlParameter("@Description",(object)model.Description??DBNull.Value),
                new SqlParameter("@CreatedBy", model.CreatedBy),
                new SqlParameter("@Contents", model.Contents),
                new SqlParameter("@Status", (object)model.Status??DBNull.Value),
                new SqlParameter("@Tags",(object)model.Tags??DBNull.Value),
                new SqlParameter("@Type", model.Type),
            };
            context.Database.ExecuteSqlCommand("Sp_News_Insert @CategoryId,@MetaTitle,@LinkSeo,@Image,@Description,@CreatedBy,@Contents,@Status,@Tags,@Type", parameters);
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
        public void Delete(int id)
        {
            var obj = Find(id);
            context.News.Remove(obj);
            context.SaveChanges();
        }

        public void Edit(News model)
        { 
            object[] parameters =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@CategoryId", model.CategoryId),
                new SqlParameter("@MetaTitle", model.MetaTitle),
                new SqlParameter("@LinkSeo", model.LinkSeo),
                new SqlParameter("@Image", (object)model.Image??DBNull.Value),
                new SqlParameter("@Description", (object)model.Description??DBNull.Value),
                new SqlParameter("@ModifiedBy", 2),
                new SqlParameter("@Contents", model.Contents),
                new SqlParameter("@Status", (object)model.Status??DBNull.Value),
                new SqlParameter("@Tags",(object)model.Tags??DBNull.Value)
            };
            context.Database.ExecuteSqlCommand("Sp_News_Update @ID,@CategoryId,@MetaTitle,@LinkSeo,@Image,@Description,@ModifiedBy,@Contents,@Status,@Tags", parameters);
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

        public ListNews Detail(int id)
        {
            return context.Database.SqlQuery<ListNews>("Sp_News_Detail @ID", new SqlParameter("@ID", id)).FirstOrDefault();
        }

        public List<ListNews> NewsGetByCategory(string linkseo)
        {
            return context.Database.SqlQuery<ListNews>("Sp_News_GetByCategory @LinkSeo", new SqlParameter("@LinkSeo", linkseo)).ToList();
        }
    }
}
