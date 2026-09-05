using CMS.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Web.Model;

namespace CMS.Reporitory
{
    public class HeaderRepository : IHeaderRepository
    {
        private readonly WebDuLichEntities context = new WebDuLichEntities();

        public void Add(string content)
        {
            context.Database.ExecuteSqlCommand("Sp_Header_Insert @Contents", new SqlParameter("@Contents", content));
        }
        public IEnumerable<Header> GetAll()
        {
            return context.Headers;
        }
         
        public void Edit(Header model)
        {
            object[] parameters =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@Contents", model.Contents)
            };
            context.Database.ExecuteSqlCommand("Sp_Header_Update @ID,@Contents", parameters);
        }

        public Header Find(int id)
        {
            return context.Headers.Find(id);
        }
    }
}
