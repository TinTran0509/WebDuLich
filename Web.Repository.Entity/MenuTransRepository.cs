using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository.Entity
{
    public class MenuTransRepository : IMenuTransRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private const string KeyCache = "MenuTrans";
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        public void Add(MenuTran model)
        {
            _entities.MenuTrans.Add(model);
            _entities.SaveChanges();
        }

        public void Create(MenuTran obj)
        {
            try
            { 
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                  
                    parameters.Add("Name", obj.Name);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("MenuID", obj.MenuID);
                    parameters.Add("ParentID", obj.ParentID);
                    parameters.Add("Ordering", obj.Ordering);
                    parameters.Add("LangCode", obj.LangCode);
                    conn.Execute("Sp_MenuTrans_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                } 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.MenuTrans.Remove(obj);
            _entities.SaveChanges();
        }
        public void Edit(MenuTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE MenuTrans ");
                    sb.Append("SET Name = @Name,LinkSeo = @LinkSeo,ParentID = @ParentID,");
                    sb.Append("Level = @Level,Ordering = @Ordering ");
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Name", obj.Name);
                    parameters.Add("LinkSeo", obj.LinkSeo);
                    parameters.Add("ParentID", obj.ParentID);
                    parameters.Add("Level", obj.Level);
                    parameters.Add("Ordering", obj.Ordering); 
                    conn.Execute(sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public MenuTran Find(int id)
        {
            return _entities.MenuTrans.Find(id);
        }

        public IEnumerable<MenuTran> GetByMenuId(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT mt.*, l.LangName AS Language FROM MenuTrans mt ");
                    sb.Append("JOIN tbl_Languages l ON mt.LangCode = l.LangCode ");
                    sb.Append("WHERE MenuID = @MenuID ORDER BY mt.ID"); 
                    var parameters = new DynamicParameters();
                    parameters.Add("MenuID", id);
                    return conn.Query<MenuTran>(
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

        public IEnumerable<MenuTranModel> GetByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT mt.*,l.LangName FROM MenuTrans mt ");
                    sb.Append("JOIN tbl_Languages l ON mt.LangCode = l.LangCode ");
                    sb.Append("WHERE mt.LangCode = @LangCode ORDER BY mt.Ordering");
                    var parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);
                    return conn.Query<MenuTranModel>(
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

        public IEnumerable<MenuTran> GetAll()
        {
            return _entities.MenuTrans;
        }

        public IEnumerable<MenuTranModel> GetByPage(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT m.ID,mt.Name,l.LangName,mt.Level,mt.Ordering FROM Menu m ");
                    sb.Append("JOIN MenuTrans mt ON mt.MenuID = m.ID ");
                    sb.Append("JOIN tbl_Languages l ON mt.LangCode = l.LangCode ");
                    if(!string.IsNullOrEmpty(langCode))
                       sb.Append("WHERE mt.LangCode = @LangCode ");
                    sb.Append("GROUP BY m.ID, mt.Name,l.LangName,mt.Level,mt.Ordering");

                    if (!string.IsNullOrEmpty(langCode))
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("LangCode", langCode);
                        return conn.Query<MenuTranModel>(
                            sb.ToString(),
                            parameters,
                            commandType: CommandType.Text);
                    }
                    else
                    { 
                        return conn.Query<MenuTranModel>(
                            sb.ToString(),
                            null,
                            commandType: CommandType.Text);
                    } 
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
