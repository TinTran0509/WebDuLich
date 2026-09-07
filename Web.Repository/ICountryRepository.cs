using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface ICountryRepository
    {
        IEnumerable<CountryViewModel> GetAll();
        IEnumerable<CountryTran> GetAllCountryTrans();
        void Add(CountryTran obj);
        int Create(Country model);
        void Delete(int id);
        Country Find(int id);
        void Edit(Country model);
        void Update(CountryTran obj);
        IEnumerable<CountryUser> GetByHeader();
        IEnumerable<CountryTran> GetCountryTranByLangCode(string langCode);
        IEnumerable<CountryTran> GetCountryTranByCountryID(int countryID);
        IEnumerable<CountryViewModel> GetCountryViewModelByLangCode(string langCode);
    }
}
