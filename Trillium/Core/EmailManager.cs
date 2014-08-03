namespace Trillium.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net.Mail;
    using System.Web.Hosting;
    using RazorEngine;
    using Encoding = System.Text.Encoding;

    public class EmailManager
    {
        private readonly string _mailFromAddress;

        private readonly string _mailFromName;
        private readonly string _smtpServer;

        private readonly string _testMailFromAddress;

        private readonly string _testMailToAddress;
        private readonly bool _testMode;

        private readonly string _testSmtpServer;


        public EmailManager()
        {
            _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "127.0.0.1";
            _mailFromAddress = ConfigurationManager.AppSettings["MailFromAddress"] ?? "noreply@domain.com";
            _mailFromName = ConfigurationManager.AppSettings["MailFromName"] ?? "MyDomain.com";
            _testMode = Convert.ToBoolean(ConfigurationManager.AppSettings["TestMode"] ?? "false");
            _testSmtpServer = ConfigurationManager.AppSettings["TestSmtpServer"] ?? "127.0.0.1";
            _testMailFromAddress = ConfigurationManager.AppSettings["TestMailFromAddress"] ?? "noreply@domain.com";
            _testMailToAddress = ConfigurationManager.AppSettings["TestMailToAddress"] ?? "luchen_sv@msn.com";
        }

        public void SendMail(string toAddress, string subject, string templateName, dynamic emailModelData)
        {
            SendMail(toAddress, subject, templateName, emailModelData, _mailFromAddress, _mailFromName);
        }

        public void SendMail(string toAddress, string subject, string templateName, dynamic emailModelData,
            string fromAddress, string fromName)
        {
            var mac = new MailAddressCollection {toAddress};

            if (string.IsNullOrEmpty(fromName))
            {
                fromName = _mailFromName;
            }

            SendMail(mac, subject, templateName, emailModelData, new MailAddress(fromAddress, fromName), null);
        }

        public void SendMail(MailAddressCollection toAddresses, string subject, string templateName,
            dynamic emailModelData, MailAddress fromAddress, IEnumerable<string> attachments)
        {
            var mm = new MailMessage();
            try
            {
                if (fromAddress == null)
                {
                    fromAddress = new MailAddress(_mailFromAddress, _mailFromName);
                }

                mm.IsBodyHtml = true;
                mm.Priority = MailPriority.Normal;
                mm.Subject = subject;

                var path = HostingEnvironment.MapPath(@"\Email\" + templateName + ".cshtml");
                if (path != null)
                {
                    var template = File.ReadAllText(path, Encoding.Default);
                    mm.Body = Razor.Parse(template, emailModelData);
                }

                SmtpClient smtp;
                if (_testMode)
                {
                    mm.To.Clear();
                    mm.From = new MailAddress(_testMailFromAddress);
                    mm.To.Add(_testMailToAddress);
                    smtp = new SmtpClient(_testSmtpServer);
                    mm.Body = "<p>Test Mode E-mail From:" + fromAddress.Address + " To:" + toAddresses[0].Address
                              + "</p>" + mm.Body;
                }
                else
                {
                    mm.From = fromAddress;
                    foreach (var toAddress in toAddresses)
                    {
                        mm.To.Add(toAddress);
                    }

                    smtp = new SmtpClient(_smtpServer);
                }

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mm.Attachments.Add(new Attachment(attachment));
                    }
                }

                smtp.Send(mm);
            }
            finally
            {
                mm.Dispose();
            }
        }
    }
}