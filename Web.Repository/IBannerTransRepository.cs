using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IBannerTransRepository
    {
        IEnumerable<BannerTran> GetAll();
        BannerTran Find(int id);
        void Add(BannerTran obj);
        void Edit(BannerTran obj);
        void Update(BannerTran obj);
        void Delete(int id);
        IEnumerable<BannerModel> GetByBannerID(int bannerID);
    }
}
