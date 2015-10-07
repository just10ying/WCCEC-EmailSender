using System.IO;
using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    class EmailData
    {
        private const string MAIL_ADDRESS_SUFFIX = "@gmail.com";
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;

        public string recipientEmail;
        public string subject;
        public string body;
        public string fileName;
        public byte[] fileContents;

        public EmailData(PDFData pdfData)
        {
            fileContents = pdfData.GetByteContents();
            fileName = pdfData.GetFileName();
        }

        public void Send(string username, string password)
        {
            MailAddress mailfrom = new MailAddress(username + MAIL_ADDRESS_SUFFIX);
            MailAddress mailto = new MailAddress(recipientEmail);
            MailMessage newmsg = new MailMessage(mailfrom, mailto);
            newmsg.Subject = subject;
            newmsg.Body = body;

            Attachment att = new Attachment(new MemoryStream(fileContents), fileName);
            newmsg.Attachments.Add(att);

            SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.EnableSsl = true;
            smtp.Send(newmsg);
        }
    }
}
