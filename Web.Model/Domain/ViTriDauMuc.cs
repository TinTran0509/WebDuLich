using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Web.Model.Domain
{
    public enum ViTriDauMuc
    {
        [Display(Name = "Menu Top")]
        MenuTop = 1,
        [Display(Name = "Danh Mục Top")]
        DanhMucTop = 2,
        [Display(Name = "Danh Mục Trái")]
        DanhMucTrai = 3,
        [Display(Name = "Giữa trang")]
        GiuaTrang = 4,
        [Display(Name = "Menu Footer")]
        MenuFooter = 5
    }
}
