namespace Trillium.Core
{
    using System.Configuration;
    using Trillium.ViewModels;

    public class EmailDispatcher
    {
        public static void SendContactUsEmail(ContactViewModel model)
        {
            var em = new EmailManager();
            string toEmailAddress = ConfigurationManager.AppSettings["ContactUsEmailAddress"] ?? "luchen_sv@msn.com";
            em.SendMail(toEmailAddress, "Website contact us", "EmailContactUs", model);
        }

        public static void SendSystemAlert(string warning)
        {
            var em = new EmailManager();
            string toEmailAddress = ConfigurationManager.AppSettings["SystemAlertEmailAddress"] ?? "luchen_sv@msn.com";
            em.SendMail(toEmailAddress, "System Alert", "EmailSimple", new { Text = warning });
        }
    }
}