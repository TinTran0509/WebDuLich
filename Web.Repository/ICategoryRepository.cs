using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        IEnumerable<CategoryTran> GetAllTrans();
        int Add(Category model);
        void AddTrans(CategoryTran model);
        void Delete(int id);
        Category Find(int id);
        void Edit(Category model);
        void EditTrans(CategoryTran obj);
        IEnumerable<CategoryModel> GetCategoryTranByCategoryID(int categoryID);
    }
}
