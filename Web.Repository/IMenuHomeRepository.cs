using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IMenuHomeRepository
    {
        IEnumerable<MenuHome> GetAll();
        IEnumerable<MenuHome> GetByLangCode(string langCode);
        MenuHome Find(int id);
        void Add(MenuHome obj);
        void Edit(MenuHome obj);
        void Delete(int id);
    }
}
