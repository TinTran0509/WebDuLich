using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface IWordRepository
    {
        IEnumerable<WordViewModel> GetAll();
        void Add(WordTran obj);
        int Create(Word model);
        void Delete(int id);
        Word Find(int id);
        void Edit(Word model);
        void Update(WordTran obj); 
        IEnumerable<WordTran> GetWordTranByLangCode(string langCode); 
        IEnumerable<WordViewModel> GetWordViewModelByLangCode(string langCode);
        IEnumerable<WordTran> GetAllWordTrans();
        string GetValueByKey(string keyName, string langCode);
    }
}
