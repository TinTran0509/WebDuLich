using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Web.Model;

namespace Web.Templates
{
    public class ContentMail
    {
        public static string ContentMailBooking(Booking booking)
        {
            string hoten = booking.Title + "" + booking.FirstName + " " + booking.LastName;
            StringBuilder sb = new StringBuilder();

            sb.Append("<div style=\"padding-left: 4px;\">");
            sb.Append($"<p style=\"font-size:12pt;\"><strong><span style=\"font-family:'Times New Roman';\">Thông báo booking tour,</span></strong></p>");
            sb.Append($"<p style=\"font-size:12pt;\"><span style=\"font-family:'Times New Roman';\">Thông tin khách hàng:</span></p>");
            sb.Append($"<ul type=\"disc\" style=\"padding-left: 24pt;\">");
            sb.Append($"<li style=\"font-family:serif; font-size:10pt;\"><strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">Khách hàng</span></strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">: {hoten}</span></li>");

            sb.Append($"<li style=\"font-family:serif; font-size:10pt;\"><strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">Số điện thoại:</span></strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">&nbsp;{booking.Tel}</span></li>");

            sb.Append($"<li style=\"font-family:serif; font-size:10pt;\"><strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">Email:</span></strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">&nbsp;{booking.Email}</span></li>");

            sb.Append($"<li style=\"font-family:serif; font-size:10pt;\"><strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">Thời gian đặt:</span></strong><span style=\"font-family:'Times New Roman'; font-size:12pt;\">&nbsp;{booking.CreatedDate.Value.ToString("dd/MM/yyy HH:mm")}</span></li>");

            sb.Append("</ul>");

            sb.Append("<p style=\"font-size:12pt;\"><span style=\"font-family:'Times New Roman';font-style: italic;\">Lưu ý</span><span style=\"font-family:'Times New Roman';\">:&nbsp;Đây là email tự động không trả lời email này.</span></p>");

            sb.Append("<p style=\"font-size:12pt;\"><span style=\"font-family:'Times New Roman';\">Truy cập nhanh:</span><strong><span style=\"font-family:'Times New Roman';color:#234595;\"><a href=\"https://pasoseatours.com/admin\">&nbsp;Click here</a></span></strong></p>");
            sb.Append("<p style=\"font-size:12pt;\"><span style=\"font-family:'Times New Roman';\">Trân trọng,</span></p>");
            sb.Append("<p style=\"font-size:12pt;\"><strong><span style=\"font-family:'Times New Roman';\">Chuyên gia du lịch châu Á</span></strong></p>");
            sb.Append("</div>");

            return sb.ToString();
        }
    }
}