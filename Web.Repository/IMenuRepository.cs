using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IMenuRepository
    {
        IEnumerable<Menu> GetAll();
        IEnumerable<MenuTranModel> GetByLangCode(string langCode);
        Menu Find(int id);
        int Add(Menu obj); 
        void Delete(int id, List<MenuTran> menuTrans);
    }
}
