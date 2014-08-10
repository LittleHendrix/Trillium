namespace Trillium.Extensions
{
    using System.Web.Mvc;
    using umbraco;

    public static class HtmlExtensions
    {
        public static MvcHtmlString Truncate(this HtmlHelper html, string value, int count, string elipses)
        {
            if (string.IsNullOrEmpty(value))
            {
                return MvcHtmlString.Empty;
            }

            var cleanHtml = library.StripHtml(value);

            return cleanHtml.Length <= count ? MvcHtmlString.Create(cleanHtml) : MvcHtmlString.Create(cleanHtml.Substring(0, count - 1) + elipses);
        }
    }
}