using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface ISliderRepository
    {
        List<Slider> GetAll();
        Slider Find(int id);
        void Add(Slider obj);
        void Edit(Slider obj);
        void Delete(int id);
    }
}
