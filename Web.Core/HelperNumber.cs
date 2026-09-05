using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Core
{
    public class HelperNumber
    {
        /// <summary>
        /// Kiểm tra số có phải số chẵn hay không ?
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsEvenNumber(int number)
        {
            return number % 2 == 0;
        }
        /// <summary>
        /// Kiểm tra có phải số lẻ hay không
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsOddNumber(int number)
        {
            return !IsEvenNumber(number);
        }
        /// <summary>
        /// Lấy ngẫu nhiên một số trong một khoảng
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int min, int max)
        {
            var random = new Random();
            if (min >= max) { throw new Exception("Min value is greater than or equal to the max value"); }
            var r = random.Next(min, max);
            return r;
        }
        /// <summary>
        /// kiểm tra một chuỗi đưa ra có phải là số hay không?
        /// </summary>
        /// <param name="numberAsString"></param>
        /// <returns></returns>
        public static bool IsNumber(string numberAsString)
        {
            if (string.IsNullOrEmpty(numberAsString)) return false;
            numberAsString = numberAsString.Trim();
            double numberTest;
            var isNumber = double.TryParse(numberAsString, out numberTest);
            return isNumber;
        }
        /// <summary>
        /// Kiểm tra xem value có được check hay không
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsCheck(int source, int target)
        {
            return (source & target) == target;
        }
        /// <summary>
        /// Chuyển đổi số sang kiểu tiền tệ
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertToMoney(object number)
        {
            return number == null ? null : Convert.ToInt32(number).ToString("#,##0");
        }

        public static decimal ConvertMoneyToDecimal(string money)
        {
            if (string.IsNullOrEmpty(money))
            {
                return 0;
            }
            return Convert.ToDecimal(money.Replace(",", string.Empty));
        }
    }
}