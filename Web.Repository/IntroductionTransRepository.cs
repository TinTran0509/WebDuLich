using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IIntroductionTransRepository
    {
        IEnumerable<IntroductionTran> GetAll();
        IntroductionTran Find(int id);
        void Add(IntroductionTran obj);
        void Edit(IntroductionTran obj);
        IEnumerable<IntroductionViewModel> GetByIntroductionID(int introductionID);
        IntroductionViewModel GetByLangCode(string langCode);
    }
}
