using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface ILanguageRepository
    {
        IEnumerable<tbl_Languages> GetAll();
        IEnumerable<tbl_Languages> GetByActive();
        void Add(tbl_Languages model);
        void Delete(int id);
        tbl_Languages Find(int id);
        void Edit(tbl_Languages model);
    }
}
