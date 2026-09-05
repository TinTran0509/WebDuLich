using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model
{
    public class NewsFooter
    {
        public string CategoryTitle { get; set; }
        public string LinkSeo { get; set; }
        public List<News> Post { get; set; }
    }
}
