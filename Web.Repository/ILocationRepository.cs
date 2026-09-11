using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface ILocationRepository
    {
        IEnumerable<LocationViewModel> GetAll();
        IEnumerable<LocationTran> GetAllLocationTrans();
        void Add(LocationTran obj);
        int Create(Location model);
        void Delete(int id);
        Location Find(int id);
        void Edit(Location model);
        void Update(LocationTran obj);
        IEnumerable<LocationViewModel> GetByLocationIDs(string locationIDs, string langCode);
        IEnumerable<LocationTran> GetLocationTranByCoutryID(string langCode, string countryIDs);
    }
}
