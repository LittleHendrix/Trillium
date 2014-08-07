// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanonicalUrlExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The canonical url extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Extensions
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// The canonical url extensions.
    /// </summary>
    public static class CanonicalUrlExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The canonical url.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString CanonicalUrl(this HtmlHelper html)
        {
            Uri rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;

            return rawUrl == null ? MvcHtmlString.Empty : CanonicalUrl(html, string.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The canonical url.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        private static MvcHtmlString CanonicalUrl(this HtmlHelper html, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Uri rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;

                if (rawUrl == null)
                {
                    return MvcHtmlString.Empty;
                }

                path = string.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath);
            }

            path = path.ToLower();

            if (path.Count(c => c == '/') > 3)
            {
                path = path.TrimEnd('/');
            }

            if (path.EndsWith("/index"))
            {
                path = path.Substring(0, path.Length - 6);
            }

            var canonical = new TagBuilder("link");
            canonical.MergeAttribute("rel", "canonical");
            canonical.MergeAttribute("href", path);

            return new MvcHtmlString(canonical.ToString(TagRenderMode.SelfClosing));
        }

        #endregion
    }
}