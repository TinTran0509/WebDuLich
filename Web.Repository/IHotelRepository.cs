using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IHotelRepository
    {
        IEnumerable<HotelViewModel> GetAll();
        IEnumerable<HotelTran> GetAllHotelTrans();
        void Add(HotelTran obj);
        int Create(Hotel model);
        void Delete(int id);
        Hotel Find(int id);
        void Edit(Hotel model);
        void Update(HotelTran obj);
        IEnumerable<HotelTran> GetByHotelID(string hotelIDs, string langCode);
    }
}
