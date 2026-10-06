using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.Commons
{
    public class SendMailHelper
    {
        private readonly string zimbraEmail = ConfigurationManager.AppSettings["ZimbraEmail"];
        private readonly string host = ConfigurationManager.AppSettings["host"];
        private readonly string port = ConfigurationManager.AppSettings["port"];
        private readonly string username = ConfigurationManager.AppSettings["username"];
        private readonly string password = ConfigurationManager.AppSettings["password"];
        public static bool RemoteServerCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public bool SendMail(
            List<string> _lstAddress,
            List<string> _lstCCAddress,
            string _subject,
            string _messageBody,
            List<string> fileAtachList,
            string strMailSendDisplayName,
            out string messs)
        {
            try
            {
                string _toMail = _lstAddress == null ? "" : $"toMail {string.Join(";", _lstAddress)}";
                string _ccMail = _lstCCAddress == null ? "" : $"ccMail {string.Join(";", _lstCCAddress)}";

                if (_lstAddress == null)
                {
                    messs = "Không có địa chỉ email người nhận";
                    return false;
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(username, strMailSendDisplayName);
                if (_lstAddress != null)
                {
                    string _mailTo = "";
                    foreach (string _toAddress in _lstAddress)
                    {
                        mail.To.Add(_toAddress);
                        _mailTo += _toAddress + ";";
                    }
                }

                if (_lstCCAddress.Any())
                {
                    string _mailCC = "";
                    foreach (string _ccAddress in _lstCCAddress)
                    {
                        mail.CC.Add(_ccAddress);
                        _mailCC += _ccAddress + ";";
                    }
                }

                //Tài liệu đính kèm
                if (fileAtachList != null)
                {
                    foreach (string path in fileAtachList)
                    {
                        Attachment attachment;
                        attachment = new Attachment(path);
                        mail.Attachments.Add(attachment);
                    }
                }

                mail.Subject = _subject;
                mail.IsBodyHtml = true;
                mail.Body = _messageBody;

                //Added this line here
                if (ZimbraEmail(zimbraEmail))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Host = host;  
                smtp.Port = Convert.ToInt32(port);     
                smtp.EnableSsl = true; //1: true; 0:false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                mail.Priority = MailPriority.High;
                smtp.Send(mail);

                smtp.Dispose();
                messs = "Send email Success! " + mail.From.Address + " to " + mail.To.ToString() + " cc " + mail.CC.ToString();
                return true;
            }
            catch (Exception ex)
            {
                messs = "Send email Failed: " + ex.Message;
                return false;
            }
        }

        private static AlternateView CreateHtmlMessage(string message, string logoPath)
        {
            var inline = new LinkedResource(logoPath);
            inline.ContentId = Guid.NewGuid().ToString();
            string htmlBody = message + @"<img src='cid:" + inline.ContentId + @"'/>";
            var alternateView = AlternateView.CreateAlternateViewFromString(
                                    htmlBody,
                                    Encoding.UTF8,
                                    MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(inline);

            return alternateView;
        }

        private static bool Valid(string email)
        {
            string match = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex reg = new Regex(match);
            if (reg.IsMatch(email))
                return true;
            else
                return false;
        }

        private static bool ZimbraEmail(string zimbraEmail)
        {
            try
            {
                string strZimbraEmial = "0";
                if (!string.IsNullOrEmpty(zimbraEmail))
                {
                    strZimbraEmial = zimbraEmail;
                }
                return strZimbraEmial.Equals("1");
            }
            catch (Exception)
            {

            }
            return false;
        }
    }
}