using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        void Add(Category model);
        void Delete(int id);
        Category Find(int id);
        void Edit(Category model);
    }
}
