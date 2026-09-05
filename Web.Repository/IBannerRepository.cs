using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IBannerRepository
    {
        IEnumerable<BannerModel> GetByPage(int pageIndex, int pageSize, out int total);
        IEnumerable<Banner> GetAll();
        Banner Find(int id);
        int Add(Banner obj);
        void Create(BannerModel obj);
        void Edit(Banner obj);
        void Delete(int id, List<BannerTran> bannerTrans);
    }
}
