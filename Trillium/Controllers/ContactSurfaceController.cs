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

            EmailDispatcher.SendContactEmail(model);

            try
            {
                if ((this.TempData.ContainsValue("true") == false)
                    || (this.TempData.ContainsKey("FormCompleted") == false))
                {
                    this.TempData.Add("FormCompleted", "true");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    ex.Message + " containsValue=" + this.TempData.ContainsValue("true") + " containsKey="
                    + this.TempData.ContainsKey("FormCompleted"));
            }

            return this.RedirectToCurrentUmbracoPage();
        }
    }
}