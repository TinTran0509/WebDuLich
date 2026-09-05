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
    public class AdvImageRepository : IAdvImageRepository
    {
        readonly BearShopEntities _entities = new BearShopEntities();
        private const string KeyCache = "cacheadminmenu"; 
        public void Add(AdvImag obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.AdvImags.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            HelperCache.RemoveCache(KeyCache);
            var obj = Find(id);
            _entities.AdvImags.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(AdvImag model)
        {
            HelperCache.RemoveCache(KeyCache);
            object[] parameters =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@Title",(object)model.Title??DBNull.Value),
                new SqlParameter("@Image",model.Image),
                new SqlParameter("@Link",(object)model.Link??DBNull.Value),
                new SqlParameter("@Position", model.Position),
                new SqlParameter("@Type", model.Type),
                new SqlParameter("@TagetBlank", model.TagetBlank),
                new SqlParameter("@DisplayOrder", model.DisplayOrder),
                new SqlParameter("@Status", model.Status)
            };
            _entities.Database.ExecuteSqlCommand("Sp_Adv_Image_Update @ID,@Title,@Image,@Link,@Position,@Type,@TagetBlank,@DisplayOrder,@Status", parameters);
        }

        public AdvImag Find(int id)
        {
            return _entities.AdvImags.Find(id);
        }

        public List<AdvImag> GetAll()
        {
            var lstData = HelperCache.GetCache<List<AdvImag>>(KeyCache);
            if (lstData == null)
            {
                lstData = _entities.AdvImags.ToList();
                HelperCache.AddCache(lstData, KeyCache);
            }
            return lstData;
        }
    }
}
