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
    public class AboutRepository : IThemeRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "Themes";

        public void Add(Theme model)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Themes.Add(model);
            _entities.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Themes.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Theme obj)
        {
            HelperCache.RemoveCache(KeyCache);
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(Theme obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE TopicTrans SET Title = @Title, LinkSeo = @LinkSeo, Description = @Description,");
                    sb.Append("Content = @Content, LangCode = @LangCode WHERE ID = @ID"); 

                    var parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Title", obj.Title);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("Description", obj.Description);
                    parameters.Add("Contents", obj.Contents);
                    parameters.Add("LangCode", obj.LangCode);

                    conn.Execute(
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

        public Theme Find(int id)
        {
            return _entities.Themes.Find(id);
        }
        
        public IEnumerable<Theme> GetAll()
        {
            return _entities.Themes;
        }

        public IEnumerable<Theme> GetByPage(string keyWord, int pageIndex, int pageSize, out int total)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                { 
                    var parameters = new DynamicParameters();
                    parameters.Add("Keyword", keyWord);
                    parameters.Add("PageIndex", pageIndex);
                    parameters.Add("PageSize", pageSize);

                    IEnumerable<Theme> lst = conn.Query<Theme>(
                            "Sp_Theme_GetPage",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                    var pageAdminMenu = lst.ToList();
                    total = pageAdminMenu.Any() ? pageAdminMenu.FirstOrDefault().TotalCount : 0;
                    return pageAdminMenu;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
