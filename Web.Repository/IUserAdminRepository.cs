using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IUserAdminRepository
    {
        IEnumerable<UserAdmin> GetAll();
        UserAdmin Find(int id);
        void Add(UserAdmin obj);
        void Edit(UserAdmin obj);
        void Edit(List<UserAdmin> lstobj);
        void Delete(int id);
    }
}
