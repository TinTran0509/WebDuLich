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
    public class MenuHomeRepository : IMenuHomeRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "MenuHome";
        public void Add(MenuHome model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.MenuHomes.Add(model);
            _entities.SaveChanges();
        }
        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.MenuHomes.Remove(obj);
            _entities.SaveChanges();
        }
        public void Edit(MenuHome model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(model).State = EntityState.Modified;
            _entities.SaveChanges();
        }
        public MenuHome Find(int id)
        {
            return _entities.MenuHomes.Find(id);
        }
        
        public IEnumerable<MenuHome> GetAll()
        {
            return _entities.MenuHomes;
        }

        public IEnumerable<MenuHome> GetByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT mt.*, l.LangName FROM MenuTrans mt ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = mt.LangCode ");
                    sb.Append("WHERE mt.LangCode = @LangCode ORDER BY mt.Ordering");

                    var parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);
                    return conn.Query<MenuHome>(
                        sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    } 
}
