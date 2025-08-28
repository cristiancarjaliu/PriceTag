using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public static class EmailSender
    {
        public static void SendEmail(string to, string subject, string body)
        {
            SmtpClient client = new("mail.saiury.ro", 587)
            {
                Credentials = new System.Net.NetworkCredential("office@saiury.ro", "@XZd2l5RN&"),
                EnableSsl = true
            };
            MailMessage mail = new("office@saiury.ro", to, subject, body);
            client.Send(mail);
        }
    }
}
