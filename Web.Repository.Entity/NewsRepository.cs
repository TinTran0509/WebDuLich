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
