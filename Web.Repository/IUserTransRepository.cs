using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IUserTransRepository
    {
        IEnumerable<UserTran> GetAll();
        UserTran Find(int id);
        void Add(UserTran obj);
        int Create(UserTran obj);
        void Edit(UserTran obj);
        void Edit(List<UserTran> lstobj);
        void Delete(int id);
        IEnumerable<UserModel> GetByUserID(int userID);
        UserModel GetByCountry(int countryID, string langCode);
    }
}
