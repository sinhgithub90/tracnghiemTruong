using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace SpaWebApp.Services
{
    public class MailjetService : IEmailSender
    {
        public async Task SendEmailAsync(string mail, string subject, string htmlMessage)
        {
            MailjetClient client = new MailjetClient(
             "46e60a3a8d7257097c29be834191fc19",
             "073b7875beaa9e541eb56222c9770295");

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            };

            // construct your email with builder
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("thihocky@qnu.edu.vn", "Phòng Khảo Thí Đại học Quy Nhơn"))
                .WithSubject(subject)
                .WithHtmlPart(htmlMessage)
                .WithTo(new SendContact(mail))
                .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email);

            // check response
            //Assert.AreEqual(1, response.Messages.Length);
        }
    }
}
