using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IProductTransRepository
    {
        IEnumerable<ProductTran> GetAll(); 
        void Add(ProductTran obj);
        void Edit(ProductTran obj); 
        IEnumerable<ProductModel> GetByProductID(int productID);
        IEnumerable<ProductModel> GetByType(int type, string langCode, int takeRow);
    }
}
