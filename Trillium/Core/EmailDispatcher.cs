namespace Trillium.Core
{
    using System.Configuration;
    using Trillium.ViewModels;

    public class EmailDispatcher
    {
        public static void SendContactEmail(ContactViewModel model)
        {
            var em = new EmailManager();
            string toEmailAddress = ConfigurationManager.AppSettings["ContactEmailAddress"] ?? "luchen_sv@msn.com";
            em.SendMail(toEmailAddress, "Website contact form", "EmailContact", model);
        }

        public static void SendSystemAlert(string warning)
        {
            var em = new EmailManager();
            string toEmailAddress = ConfigurationManager.AppSettings["SystemAlertEmailAddress"] ?? "luchen_sv@msn.com";
            em.SendMail(toEmailAddress, "System Alert", "EmailSimple", new { Text = warning });
        }
    }
}