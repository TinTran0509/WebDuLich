using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IPackageRepository
    {
        IEnumerable<Package_Price> GetAllPackagePrice();
        void AddPackagePrice(Package_Price obj);
        void UpdatePackagePrice(Package_Price obj);
        void TrunCatePackage();
    }
}
