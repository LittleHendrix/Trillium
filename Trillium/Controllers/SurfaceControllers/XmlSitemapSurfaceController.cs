namespace Trillium.Controllers
{
    using System.Text;
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;

    public class XmlSitemapSurfaceController : SurfaceController
    {
        [OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
            return this.PartialView("XmlSitemapPartial", CurrentPage);
        }
    }
}