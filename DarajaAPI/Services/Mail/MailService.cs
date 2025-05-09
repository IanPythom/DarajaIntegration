using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp; // explicitly using MailKit's SmtpClient, which does have the Disconnect method.
using MailKit.Security;
//using System.Net.Mail; // Removed since it contains an SmtpClient class that does not have a Disconnect method
using System.IO;
using DarajaAPI.Models.Domain;
using DarajaAPI.Services.Mail;

namespace MailServiceAPI.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly MailSetting _mailSettings;
        public MailService(IOptions<MailSetting> mailSettings) // Like this, we will be able to access the data from the JSON at runtime
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            // Creates a new object of MimeMessage and adds in the Sender
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();

            // If there are any attachments (files) in the request object, transform the file to an attachment
            // and add it to the mail message as an Attachment Object of Body Builder.
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            // Sets the HTML part of the email from the Body property of the request.
            builder.HtmlBody = mailRequest.Body;
            //add the attachment and HTML Body to the Body of the Email.
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            // Send the Message using the smpt’s SendMailAsync Method.
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            // We get the file path of our welcome template.
            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
            // read the file into a string.
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            // Replace the username and email tag with the actual data.
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            // Adds a default Subject and Body to the email.
            email.Subject = $"Welcome {request.UserName}";
            // Set the body of the email from the template string
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            // Send the mail
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
// The basic idea is to create an object of MimeMessage (a class from Mimekit )and send it using a
// SMTPClient instance(Mailkit).
