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
        List<Package> GetAll();
        IEnumerable<Package_Price> GetAllPackagePrice();
        Package Find(int id);
        void Add(Package obj);
        void AddPackagePrice(Package_Price obj);
        void UpdatePackagePrice(Package_Price obj);
        void Edit(Package obj);
        void Delete(int id);
        void TrunCatePackage();
    }
}
