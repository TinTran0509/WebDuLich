using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class EnumHelper
    {
        public enum Action
        {
            View = 1,
            Add = 2,
            Edit = 3,
            Delete = 4,
            Print = 5,
            Approved = 6
        }

        public enum UserType
        {
            Admin = 1,
            Canbo = 2,
            Binhthuong = 0
        }
        public enum TrangNoiBo
        {
            SinhVien = 1,
            GiangVien = 2,
        }
    }
}
