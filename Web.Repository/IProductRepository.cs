using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IProductRepository
    {
        IEnumerable<ProductModel> GetByPage(int pageIndex, int pageSize, out int total);
        IEnumerable<Product> GetAll();
        Product Find(int id);
        int Add(Product obj); 
        void Edit(Product obj, List<ProductTran> productTrans);
        void Delete(int id, List<ProductTran> productTrans);
    }
}
