using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Web.Core
{
    public class HelperDateTime
    {
        /// <summary>
        /// Hiển thị thứ ngày tháng tiếng việt
        /// </summary>
        /// <param name="dt">Thời gian hiện tại</param>
        /// <returns></returns>
        public static string DateTimeToVn(DateTime dt)
        {
            var thu = new[] { "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật" };
            var dayinWeek = (int)dt.DayOfWeek;
            var stringDatetime = string.Format("{0} ngày {1} tháng {2} năm {3}", thu[dayinWeek], dt.Day, dt.Month, dt.Year);
            return stringDatetime;
        }
        /// <summary>
        /// Đếm số ngày giữa 2 khoảng thời gian
        /// </summary>
        /// <param name="fromdate">Ngày bắt đầu</param>
        /// <param name="todate">Ngày kết thúc</param>
        /// <returns>Số ngày giữa 2 khoảng thời gian</returns>
        public static int CountDayBetween2Time(DateTime fromdate, DateTime todate)
        {
            // Difference in days, hours, and minutes.
            var ts = todate - fromdate;
            // Difference in days.
            var differenceInDays = ts.Days;
            return differenceInDays;
        }
        /// <summary>
        /// Convert DateTime định dạng dd/mm/yyyy sang mm/dd/yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTime(string dt)
        {
            var arrDate = dt.Split('/').Select(Int32.Parse).ToList();
            return new DateTime(arrDate[2], arrDate[1], arrDate[0]);
        }
        /// <summary>
        /// Thời gian trước
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.Now - Convert.ToDateTime(dt);
            if (span.Days > 365)
            {
                var years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format(" {0} {1} trước",
                years, "năm");
            }
            if (span.Days > 30)
            {
                var months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format(" {0} {1} trước",
                months, "tháng");
            }
            if (span.Days > 0)
                return String.Format(" {0} {1} trước",
                span.Days, "ngày");
            if (span.Hours > 0)
                return String.Format(" {0} {1} trước",
                span.Hours, "giờ");
            if (span.Minutes > 0)
                return String.Format(" {0} {1} trước",
                span.Minutes, "phút");
            if (span.Seconds > 5)
                return String.Format(" {0} giây trước", span.Seconds);
            return span.Seconds <= 5 ? "vừa xong" : string.Empty;
        }
        /// <summary>
        /// Chuyển đổi kiểu DateTime định dạng mm/dd/yyyy sang dd/MM/yyyy sang string
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ConvertDateToString(DateTime? date, string format = "dd/MM/yyyy")
        {

            if (date != null)
            {
                string Result = Convert.ToDateTime(date).ToString(format);
                return Result;
            }
            else
            {
                string Result = "";
                return Result;
            }
            //else return "";
        }
        public static DateTime ConvertDate(string s)
        {
            var format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            if (string.IsNullOrEmpty(s)) return new DateTime(1, 1, 1);
            var dateStr = s.Split('/');
            //if (format == "M/d/yyyy" || format == "MM/dd/yyyy")
            return new DateTime(Convert.ToInt32(dateStr[2]), Convert.ToInt32(dateStr[1]), Convert.ToInt32(dateStr[0]));
            //return new DateTime(Convert.ToInt32(dateStr[2]), Convert.ToInt32(dateStr[0]), Convert.ToInt32(dateStr[1]));
        }
    }
}