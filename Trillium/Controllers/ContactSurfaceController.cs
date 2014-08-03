namespace Trillium.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.UI;
    using Trillium.Core;
    using Trillium.ViewModels;
    using Umbraco.Web.Mvc;

    public class ContactSurfaceController : SurfaceController
    {
        [OutputCache(Duration = 0, VaryByParam = "none", Location = OutputCacheLocation.Any, NoStore = true)]
        [HttpPost]
        [ActionName("ContactUs")]
        public ActionResult ContactUsPost(ContactViewModel model)
        {
            if (model.Captcha != null)
            {
                // Captcha is set as a hidden field to trap bots
                this.ModelState.AddModelError("Captcha", "Captcha must be left empty");
            }

            TimeSpan diff = DateTime.UtcNow - model.SubmitDate;
            if (diff.TotalSeconds < 12)
            {
                this.ModelState.AddModelError("SubmitDate", "Your last submission is still being processed");
            }

            if (!this.ModelState.IsValid)
            {
                return this.CurrentUmbracoPage();
            }

            EmailDispatcher.SendContactUsEmail(model);

            this.TempData.Add("FormCompeted", "true");
            return this.RedirectToCurrentUmbracoPage();
        }
    }
}