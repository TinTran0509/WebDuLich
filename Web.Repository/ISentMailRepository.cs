using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface ISentMailRepository
    {
        List<SentMail> GetAll();
        SentMail Find(int id);
        IEnumerable<SentMail> FindAll();
        void Add(SentMail obj);
        void Edit(SentMail obj);
        void Delete(int id);
    }
}
