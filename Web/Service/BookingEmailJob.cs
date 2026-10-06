using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Commons;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;
using Web.Templates;

namespace Web.Service
{
    public class BookingEmailJob
    {
        private readonly ISentMailRepository sentMailRepository = new SentMailRepository();
        private readonly IBookingRepository bookingRepository = new BookingRepository();
        private readonly IParamRepository paramRepository = new ParamRepository();
        public void Execute()
        {
            SendMailHelper sendMailHelper = new SendMailHelper();
             
            string subject = "Thông báo khách hàng đặt tour"; 

            Param param = paramRepository.FindByKey("EmailTo");

            string mailTo = string.Empty;
            List<string> lstMailTo = new List<string>();

            if (param != null && param.Value != null)
            {
                lstMailTo = param.Value.Split(',').ToList();
            }

            List<string> dsMailCC = new List<string>();
            List<string> fileAtachList = new List<string>();

            string mailSendDisplayName = "Pasoseatours";

            string mess = "";

            List<SentMail> sentMails = sentMailRepository.FindAll().Where(x => x.Status == 0 && x.BookingID != null).ToList(); 

            foreach (SentMail sentMail in sentMails)
            {
                Booking booking = bookingRepository.Find((int)sentMail.BookingID);
                string content = ContentMail.ContentMailBooking(booking);
                bool result = sendMailHelper.SendMail(lstMailTo, dsMailCC, subject, content, fileAtachList, mailSendDisplayName, out mess);
                if (result) 
                {
                    sentMail.Status = 1;
                    sentMail.MailTo = string.Join(",", lstMailTo);
                    sentMail.SentDate = DateTime.Now;
                    sentMailRepository.Edit(sentMail);
                }
            }
        }
    }
}