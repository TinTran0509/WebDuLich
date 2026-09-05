using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    class lolol
    {
        [Required(ErrorMessage = "Mời bạn nhập tên tài khoản")]
        public string Username { set; get; }
        [Required(ErrorMessage = "Mời bạn nhập mật khẩu")]
        public string Password { set; get; }
        public bool Rememberme { set; get; }
    }
}
