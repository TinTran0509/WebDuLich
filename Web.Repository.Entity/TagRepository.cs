using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;

namespace Web.Repository.Entity
{
    public class TagRepository : ITagRepository
    {
        readonly LebQuaTangEntities _entities = new LebQuaTangEntities();
        private const string KeyCache = "HomeMenu";
        public void Add(Tag model)
        {
            _entities.Tags.Add(model);
            _entities.SaveChanges();
        }
        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Tags.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Tag model)
        {
            
            object[] parameters =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@LinkSeo",model.LinkSeo)
            };
            _entities.Database.ExecuteSqlCommand("Sp_Tag_Update @ID,@Name,@LinkSeo", parameters);
        }

        public Tag Find(int id)
        {
            return _entities.Tags.Find(id);
        }

        public List<Tag> GetAll()
        {
            return _entities.Database.SqlQuery<Tag>("Sp_Tag_GetAll").ToList();
        }
    }
}
