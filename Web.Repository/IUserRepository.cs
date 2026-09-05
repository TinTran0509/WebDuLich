using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IUserRepository
    {
        IEnumerable<tbl_User> GetAll();
        tbl_User Find(int id);
        void Add(tbl_User obj);
        void Edit(tbl_User obj);
        void Edit(List<tbl_User> lstobj);
        void Delete(int id);
        UserModel GetByCountry(int countryID, string langCode);
        UserModel GetByLangCode(string langCode);
        UserModel GetByLangID(int langID);
        IEnumerable<UserModel> GetAllByLangCode(string langCode);
    }
}
