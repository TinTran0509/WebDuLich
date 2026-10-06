using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IBookingRepository
    {
        IEnumerable<BookingModel> GetByPage(string keySearh, int status, int pageIndex, int pageSize, out int total);
        Booking Find(int id); 
        void Add(Booking model);
        void Create(Booking obj, SentMail mail);
        void Delete(int id);
    }
}
