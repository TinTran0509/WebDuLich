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
    public class PolicyRepository : IPolicyRepository
    {
        readonly BearShopEntities _entities = new BearShopEntities();
       
        public void Add(Policy obj)
        {
            _entities.Policies.Add(obj);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Policies.Remove(obj);
            _entities.SaveChanges();
        }
        public void Edit(Policy model)
        {
            var obj = Find(model.ID);
            obj.LinkSeo = model.LinkSeo;
            obj.MetaTitle = model.MetaTitle;
            obj.Contents = model.Contents;
            obj.Tags = model.Tags;
            _entities.SaveChanges();
        }
        //public void Edit(About model)
        //{
        //    object[] parameters =
        //    {
        //        new SqlParameter("@ID", model.ID),
        //        new SqlParameter("@MetaTitle",model.MetaTitle),
        //        new SqlParameter("@Contents", model.Contents),
        //        new SqlParameter("@Tags", (object)model.Tags?? DBNull.Value)
        //    };
        //    _entities.Database.ExecuteSqlCommand("Sp_About_Update @ID,@MetaTitle,@Contents,@Tags", parameters);
        //}
        public Policy Find(int id)
        {
            return _entities.Policies.Find(id);
        }

        public List<Policy> GetAll()
        {
            return _entities.Policies.ToList();
        }
    }
}
