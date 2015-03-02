namespace Trillium.Core
{
    using System.Net.Mime;

    using RazorEngine;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Web.Hosting;

    public class EmailManager
    {
        private readonly string smtpServer;

        private readonly int smtpPort;

        private readonly string smtpUsername;

        private readonly string smtpPassword;

        private readonly bool enableSsl;

        private readonly string mailFromAddress;

        private readonly string mailFromName;

        private readonly bool testMode;

        private readonly string testSmtpServer;

        private readonly int testSmtpPort;

        private readonly string testSmtpUsername;

        private readonly string testSmtpPassword;

        private readonly bool testEnableSsl;

        private readonly string testMailFromAddress;

        private readonly string testMailToAddress;

        public EmailManager()
        {
            this.smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "127.0.0.1";
            this.smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"] ?? "25");
            this.smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "username";
            this.smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "password";
            this.enableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"] ?? "false");
            this.mailFromName = ConfigurationManager.AppSettings["MailFromName"] ?? "Umbraco v7";
            this.mailFromAddress = ConfigurationManager.AppSettings["MailFromAddress"] ?? "noreply@mydomain.com";
            this.testMode = Convert.ToBoolean(ConfigurationManager.AppSettings["TestMode"] ?? "false");
            this.testSmtpServer = ConfigurationManager.AppSettings["TestSmtpServer"] ?? "127.0.0.1";
            this.testSmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["TestSmtpPort"] ?? "25");
            this.testSmtpUsername = ConfigurationManager.AppSettings["TestSmtpUsername"] ?? "username";
            this.testSmtpPassword = ConfigurationManager.AppSettings["TestSmtpPassword"] ?? "password";
            this.testEnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["TestEnableSsl"] ?? "false");
            this.testMailFromAddress = ConfigurationManager.AppSettings["TestMailFromAddress"] ?? "notreply@mydomain.com";
            this.testMailToAddress = ConfigurationManager.AppSettings["TestMailToAddress"] ?? "luchen_sv@msn.com";
        }

        public void SendMail(string to, string subject, string templateName, dynamic emailModelData)
        {
            this.SendMail(to, subject, templateName, emailModelData, this.mailFromAddress, this.mailFromName);
        }

        public void SendMail(
            string to,
            string subject,
            string templateName,
            dynamic emailModelData,
            string fromAddress,
            string fromName)
        {
            var addresses = new MailAddressCollection { to };

            this.SendMail(addresses, subject, templateName, emailModelData, new MailAddress(fromAddress, fromName), null);
        }

        public void SendMail(
            MailAddressCollection toAddresses,
            string subject,
            string templateName,
            dynamic emailModalData,
            MailAddress fromAddress,
            IEnumerable<string> attachments)
        {
            var message = new MailMessage();

            try
            {
                message.IsBodyHtml = true;
                message.Priority = MailPriority.Normal;

                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                var tmlPath = HostingEnvironment.MapPath(@"\Email\" + templateName + ".cshtml");
                if (tmlPath != null)
                {
                    var tml = File.ReadAllText(tmlPath, System.Text.Encoding.Default);
                    message.Body = Razor.Parse(tml, emailModalData);
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                }

                SmtpClient smtp;

                if (this.testMode)
                {
                    smtp = new SmtpClient(this.testSmtpServer, this.testSmtpPort)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(this.testSmtpUsername, this.testSmtpPassword),
                        EnableSsl = this.testEnableSsl
                    };

                    message.To.Clear();

                    message.From = new MailAddress(this.testMailFromAddress, "Sent as test");
                    message.To.Add(this.testMailToAddress);
                    message.Body += "<p>Test Mode Email: from: " + fromAddress.Address + " To: " + toAddresses[0].Address
                                   + "</p>";
                    message.BodyEncoding = System.Text.Encoding.UTF8;

                }
                else
                {
                    smtp = new SmtpClient(this.smtpServer, this.smtpPort)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(this.smtpUsername, this.smtpPassword),
                        EnableSsl = this.enableSsl
                    };

                    message.From = fromAddress;
                    foreach (var toAddress in toAddresses)
                    {
                        message.To.Add(toAddress);
                    }
                }

                if (attachments != null)
                {
                    foreach (var file in attachments)
                    {
                        var data = new Attachment(file, MediaTypeNames.Application.Octet);
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTimeUtc(file);
                        disposition.ModificationDate = File.GetLastWriteTimeUtc(file);
                        disposition.ReadDate = File.GetLastAccessTimeUtc(file);

                        message.Attachments.Add(data);
                    }
                }

                smtp.Send(message);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("SmtpException has occured: " + ex.Message);
            }
            finally
            {
                message.Dispose();
            }

        }

    }
}