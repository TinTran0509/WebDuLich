using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IThemeRepository
    {
        IEnumerable<Theme> GetAll();
        Theme Find(int id);
        void Add(Theme obj);
        void Edit(Theme obj);
        void Update(Theme obj);
        void Delete(int id);
        IEnumerable<Theme> GetByPage(string keyWord, int pageIndex, int pageSize, out int total);
    }
}
