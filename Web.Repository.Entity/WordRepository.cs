using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Web.Core;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository.Entity
{
    public class WordRepository : IWordRepository
    {
        readonly WebDuLichEntities _entities = new WebDuLichEntities();
        private readonly string _connectString = ConfigurationManager.AppSettings["ConnectionStringBackupSQL"];
        private const string KeyCache = "WordTrans";

        public void Add(WordTran obj)
        {
            _entities.WordTrans.Add(obj);
            _entities.SaveChanges();
        }

        public int Create(Word model)
        {
            try
            {
                int id = 0;
                using (var conn = new SqlConnection(_connectString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("KeyName", model.KeyName); 
                    parameters.Add("Active", model.Active); 
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("Sp_Word_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    id = parameters.Get<int>("@ID");
                }
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            var obj = Find(id);
            _entities.Words.Remove(obj);
            _entities.SaveChanges();
        }

        public void Edit(Word obj)
        {
            _entities.Entry(obj).State = EntityState.Modified;
            _entities.SaveChanges();
        }

        public void Update(WordTran obj)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE WordTrans ");
                    sb.Append("SET Value = @Value "); 
                    sb.Append("WHERE ID = @ID");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ID", obj.ID);
                    parameters.Add("Value", obj.Value); 
                    conn.Execute(sb.ToString(),
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Word Find(int id)
        {
            return _entities.Words.Find(id);
        }

        public IEnumerable<WordViewModel> GetAll()
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT w.*,wt.Value,wt.WordID,l.LangName FROM Word w ");
                    sb.Append("JOIN WordTrans wt ON wt.WordID = w.ID ");
                    sb.Append("JOIN tbl_Languages l ON l.LangCode = wt.LangCode");

                    return conn.Query<WordViewModel>(
                        sb.ToString(),
                        null,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<WordTran> GetWordTranByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                { 
                    string sql = "SELECT ct.* FROM WordTrans ct WHERE ct.LangCode = @LangCode";
                  
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode); 

                    return conn.Query<WordTran>(
                        sql,
                        parameters,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<WordViewModel> GetWordViewModelByLangCode(string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT c.Image, ct.* FROM CountryTrans ct ");
                    sb.Append("JOIN Word c ON c.ID = ct.CountryID ");
                    sb.Append("WHERE ct.LangCode = @LangCode"); 

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("LangCode", langCode);

                    return conn.Query<WordViewModel>(
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

        public IEnumerable<WordTran> GetAllWordTrans()
        {
            return _entities.WordTrans;
        }

        public string GetValueByKey(string keyName, string langCode)
        {
            try
            {
                using (var conn = new SqlConnection(_connectString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT wt.Value FROM Word w ");
                    sb.Append("JOIN WordTrans wt ON w.ID = wt.WordID "); 
                    sb.Append(" WHERE w.KeyName = @KeyName AND wt.LangCode = @LangCode");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("KeyName", keyName);
                    parameters.Add("LangCode", langCode);

                    WordTran word = conn.Query<WordTran>(
                        sb.ToString(),
                        parameters,
                        commandType: CommandType.Text).FirstOrDefault();
                    return word != null ? word.Value : "";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
