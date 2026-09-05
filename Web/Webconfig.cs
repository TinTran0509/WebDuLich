using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Core;

namespace Web
{
    public class Webconfig
    {
        public static string LangCodeVn = "vn";
        public enum UserType
        {
            Admin = 1,
            Staff = 2,
            Guest = 3
        }
        public enum SlideImages
        {
            Giua = 1,
            Phai1 = 2,
            Phai2 = 3,
            Banner = 4,
            BannerMobile = 5,
        }
        public enum Gallery
        {
            LargeImage = 1,
            SmallImage = 2
        }
        public enum NewsStatus
        {
            Nhap = 0,
        }
        
        public static int RowLimit = 2000;
        public static int RowLimitFE = 30;
    }

    public class ConfigUpload
    {
        public static string TargetUpload = "/Upload";
        /// <summary>
        /// Tính theo MB
        /// </summary>
        public static int LimitUpload = 5;
        /// <summary>
        /// Tính theo GB
        /// </summary>
        public static int LimitUploadVideo = 1;
        /// <summary>
        /// Dung resize anh
        /// </summary>
        public static int MaxWidth = 300;
        public static int MaxHeight = 300;
    }
}