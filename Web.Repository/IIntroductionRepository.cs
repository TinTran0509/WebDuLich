using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IIntroductionRepository
    {
        IEnumerable<IntroductionViewModel> GetAll();
        Introduction Find(int id);
        int Add(Introduction obj);
        void Edit(Introduction obj);
    }
}
