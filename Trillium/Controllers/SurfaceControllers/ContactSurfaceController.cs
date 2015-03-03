namespace Trillium.Controllers.SurfaceControllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.UI;
    using Trillium.Core;
    using Trillium.Extensions;
    using Trillium.ViewModels;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    public class ContactSurfaceController : SurfaceController
    {
        private readonly ContactViewModel contactViewModel;

        public ContactSurfaceController()
        {
            this.contactViewModel = new ContactViewModel();
        }

        [OutputCache(Duration = 0, VaryByParam = "none", Location = OutputCacheLocation.Any, NoStore = true)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("ContactUs")]
        public ActionResult ContactUsPost(ContactViewModel model)
        {
            TimeSpan diff = DateTime.UtcNow - model.SubmitDate;
            if (diff.TotalSeconds < 12)
            {
                this.ModelState.AddModelError("Timestamp", string.Format("Your last submission ({0}) is still being processed", model.SubmitDate));
            }

            if (!this.ModelState.IsValid)
            {
                return this.CurrentUmbracoPage();
            }

            EmailDispatcher.SendContactEmail(model);

            //this.TempData["FormCompleted"] = "true";

            //try
            //{
            //    if ((this.TempData.ContainsValue("true") == false)
            //        || (this.TempData.ContainsKey("FormCompleted") == false))
            //    {
            //        this.TempData.Add("FormCompleted", "true");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(
            //        ex.Message + " containsValue=" + this.TempData.ContainsValue("true") + " containsKey="
            //        + this.TempData.ContainsKey("FormCompleted"));
            //}

            var settings = Umbraco.GetTypedNodeByAlias("AdminSettings");
            int pageId = settings != null && settings.HasValue("thankYouPage")
                ? settings.GetPropertyValue<int>("thankYouPage")
                : 1093;

            return this.RedirectToUmbracoPage(pageId);
        }

        [ChildActionOnly]
        public ActionResult RenderContactForm()
        {
            return this.PartialView("ContactForm", this.contactViewModel);
        }
    }
}