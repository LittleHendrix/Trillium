namespace Trillium.Core
{
    using System.Configuration;
    using Trillium.ViewModels;

    public class EmailDispatcher
    {
        public static void SendContactEmail(ContactViewModel model)
        {
            var to = ConfigurationManager.AppSettings["ContactEmailAddress"] ?? "luchen_sv@msn.com";
            var em = new EmailManager();
            em.SendMail(to, "Trillium Fitness website contact", "EmailContact", model);
        }

        public static void SendSystemAlert(string warning)
        {
            var to = ConfigurationManager.AppSettings["SystemAlertEmailAddress"] ?? "luchen_sv@msn.com";
            var em = new EmailManager();
            em.SendMail(to, "System Alert", "EmailSimple", new { Text = warning });
        }
    }
}