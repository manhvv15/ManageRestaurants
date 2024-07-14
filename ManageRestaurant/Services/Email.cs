using System.Net.Mail;
using System.Net;

namespace ManageRestaurant.Services
{
    public class Email
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("manhvu152k2@gmail.com");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "pnqg kzcz jwfi kjug";   //manh20020105
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
            }
        }
    }
}
