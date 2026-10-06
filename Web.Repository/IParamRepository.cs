using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IParamRepository
    {
        List<Param> GetAll();
        Param Find(int id);
        Param FindByKey(string key);
        void Add(Param obj);
        void Edit(Param obj);
        void Delete(int id);
    }
}
