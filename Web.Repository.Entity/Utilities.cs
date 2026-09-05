using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Web.Repository.Entity
{
   public class Utilities
    {
        public static string RemoveMark(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
                var charsToRemove = new string[] { "@", ",", ";", "'", "/", "\\", "\"", "[", "]", "+", "(", ")" };
                foreach (var c in charsToRemove)
                {
                    str = str.Replace(c, string.Empty);
                }
                const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ ";
                const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY-";
                int index = -1;
                char[] arrChar = FindText.ToCharArray();
                while ((index = str.IndexOfAny(arrChar)) != -1)
                {
                    int index2 = FindText.IndexOf(str[index]);
                    str = str.Replace(str[index], ReplText[index2]);
                }

            }
            return str;
        }
        public static string GetThumbnail(string FileName)
        {
            var ext = Path.GetExtension(FileName).ToLower();
            string icon = "";
            switch (ext)
            {
                case ".doc":
                case ".docx":
                    icon = "/Images/IconFile/WordIcon.png";
                    break;
                case ".xls":
                case ".xlsx":
                    icon = "/Images/IconFile/ExcelIcon.png";
                    break;
                case ".zip":
                case ".rar":
                    icon = "/Images/IconFile/ZipIcon.png";
                    break;
                case ".pdf":
                    icon = "/Images/IconFile/PDFIcon.png";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    icon = FileName;
                    break;
                default:
                    icon = "/Images/IconFile/unknown.png";
                    break;
            }
            return icon;
        }
    }
}
