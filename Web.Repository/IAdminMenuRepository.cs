using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IAdminMenuRepository
    {
        List<AdminMenu> GetAll();
        AdminMenu Find(int id);
        void Add(AdminMenu obj);
        void Edit(AdminMenu obj);
        void Delete(int id);
    }
}
