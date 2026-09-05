using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Web.Core
{
    public class HelperHtml
    {
        /// <summary>
        /// Remove thẻ Html
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string input)
        {
            try
            {
                // these are end tags that create a line break and need to be replaced with a space
                var lineBreakEndTags = new[] { "</p>", "<br/>", "<br />", "</div>", "</table>", "<nl/>", "<nl />" };

                if (!string.IsNullOrEmpty(input))
                {
                    // replace carriage return w/ space
                    input = input.Replace("\r", " ");

                    // replace newline w/ space
                    input = input.Replace("\n", " ");

                    // replace tab
                    input = input.Replace("\t", string.Empty);

                    // replace line break tags with a space
                    input = lineBreakEndTags.Aggregate(input, (current, tag) => current.Replace(tag, " "));

                    // replace all html tags
                    var regex = new Regex("<(.|\n)*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    input = regex.Replace(input, string.Empty);

                    // replace special html characters
                    var specialChars = new Dictionary<string, string>
                    {
                        {"\u00A0", " "},
                        {@"&nbsp;", " "},
                        {@"&quot;", "\""},
                        {@"&ldquo;", "\""},
                        {@"&rdquo;", "\""},
                        {@"&rsquo;", "'"},
                        {@"&lsquo;", "'"},
                        {@"&amp;", "&"},
                        {@"&ndash;", "-"},
                        {@"&mdash;", "-"},
                        {@"&lt;", "<"},
                        {@"&gt;", ">"},
                        {@"&lsaquo;", "<"},
                        {@"&rsaquo;", ">"},
                        {@"&trade;", "(tm)"},
                        {@"&frasl;", "/"},
                        {@"&copy;", "(c)"},
                        {@"&reg;", "(r)"},
                        {@"&iquest;", "?"},
                        {@"&iexcl;", "!"},
                        {@"&bull;", "*"}
                    };

                    input = specialChars.Aggregate(input, (current, specialChar) => current.Replace(specialChar.Key, specialChar.Value));

                    // any other special char is deleted
                    input = Regex.Replace(input, @"&#[^ ;]+;", string.Empty);
                    input = Regex.Replace(input, @"&[^ ;]+;", string.Empty);

                    // remove extra duplicate spaces
                    input = Regex.Replace(input, @"( )+", " ");

                    // trim
                    input = input.Trim();
                }
            }
            catch
            {
                //Log.WriteError("zsdgsfthsaeaest", ex, "string that failed to be stripped of html: " + input);
            }
            return input;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Strip(string text)
        {
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }

        public static IEnumerable<SyndicationItem> GetNews(string urlAddress)
        {
            string WebContent = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) coc_coc_browser/60.4.136 Chrome/54.4.2840.136 Safari/537.36";
            request.Accept = "text/html";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                WebContent = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            WebContent = WebContent.Replace("<dc:description>", "<category>");
            WebContent = WebContent.Replace("</dc:description>", "</category>");
            WebContent = WebContent.Replace("<description>", "<category>");
            WebContent = WebContent.Replace("</description>", "</category>");
            WebContent = WebContent.Replace("<content:encoded>", "<description>");
            WebContent = WebContent.Replace("</content:encoded>", "</description>");
            //XmlReader reader = XmlReader.Create(new System.IO.StringReader(Content));
            XmlTextReader reader = new XmlTextReader(new StringReader(WebContent));
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            if (feed == null) return null;
            return feed.Items;
        }
    }
}