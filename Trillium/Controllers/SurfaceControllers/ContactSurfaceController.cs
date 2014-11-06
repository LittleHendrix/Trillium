namespace Trillium.Controllers.SurfaceControllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.UI;
    using Trillium.Core;
    using Trillium.ViewModels;
    using Umbraco.Web.Mvc;

    public class ContactSurfaceController : SurfaceController
    {
        private readonly ContactViewModel contactViewModel;

        public ContactSurfaceController(ContactViewModel contactViewModel)
        {
            this.contactViewModel = contactViewModel;
        }

        [OutputCache(Duration = 0, VaryByParam = "none", Location = OutputCacheLocation.Any, NoStore = true)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("ContactUs")]
        public ActionResult ContactUsPost(ContactViewModel model)
        {
            //TimeSpan diff = DateTime.UtcNow - model.Timestamp;
            //if (diff.TotalSeconds < 12)
            //{
            //    this.ModelState.AddModelError("Timestamp", string.Format("Your last submission ({0}) is still being processed", model.Timestamp));
            //}

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

        
        public ActionResult RenderContactForm()
        {
            return this.PartialView("ContactForm", this.contactViewModel);
        }
    }
}