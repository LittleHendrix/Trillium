namespace Trillium.Core
{
    using System.Configuration;
    using Trillium.ViewModels;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using umbraco.NodeFactory;

    public class EmailDispatcher
    {
        public static void SendContactEmail(ContactViewModel model)
        {
            string to = ConfigurationManager.AppSettings["ContactEmailAddress"] ?? "luchen_sv@msn.com";
            var em = new EmailManager();
            em.SendMail(to, "Trillium Fitness website contact", "EmailContact", model);

            SaveContactForm(model);
        }

        public static void SendSystemAlert(string warning)
        {
            string to = ConfigurationManager.AppSettings["SystemAlertEmailAddress"] ?? "luchen_sv@msn.com";
            var em = new EmailManager();
            em.SendMail(to, "System Alert", "EmailSimple", new {Text = warning});
        }

        private static void SaveContactForm(ContactViewModel model)
        {
            const string msgDocTypeAlias = "Message";
            const string namePropperty = "fromName";
            const string emailPropperty = "fromEmailAddress";
            const string subjectPropperty = "subject";
            const string messagePropperty = "messageBody";
            const string datetimePropperty = "submittedOn";

            Node contactNode = Node.GetNodeByXpath("//Contact[1]");

            IContentService cs = ApplicationContext.Current.Services.ContentService;

            IContent content = cs.CreateContent(model.EmailAddress, contactNode.Id, msgDocTypeAlias);
            content.Name = model.EmailAddress;
            if (content.HasProperty("fromName"))
            {
                content.SetValue(namePropperty, model.Name);
            }
            if (content.HasProperty("fromEmailAddress"))
            {
                content.SetValue(emailPropperty, model.EmailAddress);
            }
            if (content.HasProperty("subject"))
            {
                content.SetValue(subjectPropperty, model.Subject);
            }
            if (content.HasProperty("messageBody"))
            {
                content.SetValue(messagePropperty, model.Message);
            }
            if (content.HasProperty("submittedOn"))
            {
                content.SetValue(datetimePropperty, model.Timestamp.ToString("f"));
            }

            cs.Save(content);
        }
    }
}