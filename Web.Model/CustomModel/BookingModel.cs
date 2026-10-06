using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model
{
    public class BookingModel : Booking
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int TotalCount { get; set; }
    }
}
