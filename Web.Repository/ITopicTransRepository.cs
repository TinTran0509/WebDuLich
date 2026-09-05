using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;
using Web.Model.CustomModel;

namespace Web.Repository
{
    public interface ITopicTransRepository
    {
        IEnumerable<TopicTran> GetAll();
        TopicTran Find(int id);
        void Add(TopicTran obj);
        void Edit(TopicTran obj);
        void Update(TopicTran obj);
        void Delete(int id);
    }
}
