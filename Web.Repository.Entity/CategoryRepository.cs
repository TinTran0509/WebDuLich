using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;

namespace Web.Repository.Entity
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "cachecategory";
        public void Add(Category model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Categories.Add(model);
            _entities.SaveChanges();
            //object[] parameters =
            // {
            //    new SqlParameter("@Name",model.Name ),
            //    new SqlParameter("@LinkSeo",(object)model.LinkSeo?? DBNull.Value),
            //    new SqlParameter("@ParentID", model.ParentID),
            //    new SqlParameter("@Ordering", model.Ordering)
            //};
            //_entities.Database.ExecuteSqlCommand("Sp_Category_Insert @Name,@LinkSeo,@ParentID,@Ordering", parameters);
        }
        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Categories.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Category model)
        {
            HelperCache.RemoveCache(KeyCache); 
            //_entities.Entry(model).State = EntityState.Modified;
            //_entities.SaveChanges();
            object[] parameters =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@Name",model.Name ),
                new SqlParameter("@LinkSeo",model.LinkSeo),
                new SqlParameter("@ParentID", model.ParentID),
                new SqlParameter("@Ordering", model.Ordering),
                new SqlParameter("@Type", 1),
                new SqlParameter("@Level", model.Level),
                new SqlParameter("@Image", (object)model.Image ?? DBNull.Value),
                new SqlParameter("@Position", 1),
                new SqlParameter("@IsSearch", (object)model.IsSearch ?? DBNull.Value)
            };
            _entities.Database.ExecuteSqlCommand("Sp_Category_Update @ID,@Name,@LinkSeo,@ParentID,@Ordering,@Type,@Level,@Image,@Position,@IsSearch", parameters);
        }

        public Category Find(int id)
        {
            return _entities.Categories.Find(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _entities.Categories;
        }
    }
}
