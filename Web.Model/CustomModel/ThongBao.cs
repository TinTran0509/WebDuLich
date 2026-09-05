using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class ThongBao
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CommentId { get; set; }
        public int TypeId { get; set; } //(NewsId or TTHCId)
        public bool? IsView { get; set; }
        public bool Status { get; set; }
    }
}
