namespace Trillium.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Web.Hosting;
    using RazorEngine;
    using Encoding = System.Text.Encoding;

    public class EmailManager
    {
        private readonly string _mailFromAddress;

        private readonly string _mailFromName;
        private readonly string _smtpHost;

        private readonly string _testMailFromAddress;

        private readonly string _testMailToAddress;
        private readonly bool _testMode;

        private readonly string _testSmtpHost;
        private readonly int _testSmtpPort;
        private readonly string _testSmtpUsername;
        private readonly string _testSmtpPassword;


        public EmailManager()
        {
            this._smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "127.0.0.1";
            this._mailFromAddress = ConfigurationManager.AppSettings["MailFromAddress"] ?? "noreply@mydomain.com";
            this._mailFromName = ConfigurationManager.AppSettings["MailFromName"] ?? "MyDomain.com";
            this._testMode = Convert.ToBoolean(ConfigurationManager.AppSettings["TestMode"] ?? "false");
            this._testSmtpHost = ConfigurationManager.AppSettings["TestSmtpHost"] ?? "127.0.0.1";
            //this._testSmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["TestSmtpPort"] ?? "25");
            //this._testSmtpUsername = ConfigurationManager.AppSettings["TestSmtpUsername"] ?? "username";
            //this._testSmtpPassword = ConfigurationManager.AppSettings["TestSmtpPassword"] ?? "password";
            this._testMailFromAddress = ConfigurationManager.AppSettings["TestMailFromAddress"] ?? "noreply@mydomain.com";
            this._testMailToAddress = ConfigurationManager.AppSettings["TestMailToAddress"] ?? "luchen_sv@msn.com";
        }

        public void SendMail(string toAddress, string subject, string templateName, dynamic emailModelData)
        {
            SendMail(toAddress, subject, templateName, emailModelData, this._mailFromAddress, this._mailFromName);
        }

        public void SendMail(string toAddress, string subject, string templateName, dynamic emailModelData,
            string fromAddress, string fromName)
        {
            var mac = new MailAddressCollection {toAddress};

            if (string.IsNullOrEmpty(fromName))
            {
                fromName = this._mailFromName;
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
                    fromAddress = new MailAddress(this._mailFromAddress, this._mailFromName);
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
                if (this._testMode)
                {
                    //smtp = new SmtpClient(this._testSmtpHost, this._testSmtpPort)
                    //{
                    //    Credentials = new NetworkCredential(this._testSmtpUsername, this._testSmtpPassword),
                    //    EnableSsl = true
                    //};

                    smtp = new SmtpClient(this._testSmtpHost);

                    mm.To.Clear();
                    mm.From = new MailAddress(this._testMailFromAddress);
                    mm.To.Add(this._testMailToAddress);
                    mm.Body = "<p>Test Mode E-mail From:" + fromAddress.Address + " To:" + toAddresses[0].Address
                              + "</p>" + mm.Body;
                }
                else
                {
                    smtp = new SmtpClient(this._smtpHost);

                    mm.From = fromAddress;
                    foreach (var toAddress in toAddresses)
                    {
                        mm.To.Add(toAddress);
                    }
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