using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IGroupUserRepository
    {
        IEnumerable<tbl_GroupUser> GetAll();
        tbl_GroupUser Find(int id);
        void Add(tbl_GroupUser obj);
        void Edit(tbl_GroupUser obj);        
        void Delete(int id);
    }
}
