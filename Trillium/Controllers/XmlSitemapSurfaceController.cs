namespace Trillium.Controllers
{
    using System.Web.Mvc;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    public class XmlSitemapSurfaceController : SurfaceController
    {
        [OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            Response.ContentType = "text/xml";
            return this.PartialView("XmlSitemapPartial", CurrentPage);
        }
    }
}