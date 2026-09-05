using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model
{
    public partial class Category
    {
        public string ParentName { get; set; }
        public int Level1 { get; set; }
        public string DisplayOrder { get; set; }
    }
}
