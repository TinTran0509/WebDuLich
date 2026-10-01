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
        IEnumerable<ProductModel> GetByType(int type, string langCode, int productId, int takeRow);
        ProductModel GetByLinkSeo(string linkSeo);
        ProductModel GetByProductTransId(int id);
        IEnumerable<Itinerary> GetItineraryByProductID(int productID, string langCode);
        IEnumerable<ProductModel> GetRelate(int prodId, int menuId, string langCode,  int takeRow);
    }
}
