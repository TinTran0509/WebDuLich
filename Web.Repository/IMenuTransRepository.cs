using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IMenuTransRepository
    {
        IEnumerable<MenuTran> GetAll();
        MenuTran Find(int id);
        IEnumerable<MenuTran> GetByMenuId(int id);
        IEnumerable<MenuTranModel> GetByLangCode(string langCode);
        void Add(MenuTran obj);
        void Create(MenuTran obj);
        void Edit(MenuTran obj);
        void Delete(int id);
        IEnumerable<MenuTranModel> GetByPage(string langCode);
    }
}
