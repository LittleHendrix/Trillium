// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishedContentExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The published content extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Trillium.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Newtonsoft.Json.Linq;

    using RJP.MultiUrlPicker.Models;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    ///     The published content extensions.
    /// </summary>
    public static class PublishedContentExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get multi url links list.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="mupPropertyAlias">
        /// The mup property alias.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <param name="ordered">
        /// The list type.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString GetMultiUrlLinksList(
            this IPublishedContent publishedContent, 
            string mupPropertyAlias, 
            object htmlAttributes = null, 
            bool ordered = false)
        {
            if (!publishedContent.HasValue(mupPropertyAlias))
            {
                return MvcHtmlString.Empty;
            }

            var multiUrlPicker = publishedContent.GetPropertyValue<MultiUrls>(mupPropertyAlias);
            if (!multiUrlPicker.Any())
            {
                return MvcHtmlString.Empty;
            }

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) as IDictionary<string, object>;

            string listType = ordered ? "ol" : "ul";

            var htmlList = new TagBuilder(listType);
            htmlList.CustomMergeAttributes(attributes);

            foreach (Link item in multiUrlPicker)
            {
                var li = new TagBuilder("li");
                var a = new TagBuilder("a");

                a.MergeAttribute("href", item.Url, false);
                a.MergeAttribute("target", item.Target, false);

                if (item.Id == null)
                {
                    a.MergeAttribute("rel", "nofollow", false);
                }

                a.SetInnerText(item.Name);

                li.InnerHtml += a.ToString();
                htmlList.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(htmlList.ToString());
        }

        /// <summary>
        /// The get related links list.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="rlPropertyAlias">
        /// The rl property alias.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <param name="ordered">
        /// The ordered.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString GetRelatedLinksList(
            this IPublishedContent publishedContent, 
            string rlPropertyAlias, 
            object htmlAttributes = null, 
            bool ordered = false)
        {
            if (!publishedContent.HasValue(rlPropertyAlias))
            {
                return MvcHtmlString.Empty;
            }

            var helper = new UmbracoHelper(UmbracoContext.Current);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) as IDictionary<string, object>;

            string listType = ordered ? "ol" : "ul";

            var htmlList = new TagBuilder(listType);
            htmlList.CustomMergeAttributes(attributes);

            foreach (JToken item in publishedContent.GetPropertyValue<JArray>(rlPropertyAlias))
            {
                var li = new TagBuilder("li");
                var a = new TagBuilder("a");

                string linkUrl = item.Value<bool>("isInternal")
                                     ? helper.NiceUrl(item.Value<int>("internal"))
                                     : item.Value<string>("link");
                string linkTarget = item.Value<bool>("newWindow") ? "_blank" : string.Empty;
                var caption = item.Value<string>("caption");

                a.MergeAttribute("href", linkUrl, false);
                a.MergeAttribute("target", linkTarget, false);
                if (!item.Value<bool>("isInternal"))
                {
                    a.MergeAttribute("rel", "nofollow", false);
                }

                a.SetInnerText(caption);

                li.InnerHtml += a.ToString();
                htmlList.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(htmlList.ToString());
        }

        /// <summary>
        /// The images for.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="maxItems">
        /// The max items.
        /// </param>
        /// <param name="isResponsive">
        /// The is Responsive.
        /// </param>
        /// <param name="enableLink">
        /// The enable Link.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString ImagesFor(
            this IPublishedContent publishedContent, 
            string propertyAlias, 
            int maxItems = 1000, 
            bool isResponsive = false, 
            bool enableLink = false,
            bool enableCrop = false,
            string cropPropAlias = "imageCrop",
            string cropName = "thumb",
            string htmlClassName = "")
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return MvcHtmlString.Empty;
            }

            var helper = new UmbracoHelper(UmbracoContext.Current);

            IEnumerable<int> itemList =
                publishedContent.GetPropertyValue<string>(propertyAlias)
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);
            List<IPublishedContent> itemCollection = itemList.Select(helper.TypedMedia).Take(maxItems).ToList();

            if (!itemCollection.Any())
            {
                return MvcHtmlString.Empty;
            }

            string tags = string.Empty;

            foreach (IPublishedContent item in itemCollection)
            {
                var imgTag = new TagBuilder("img");
                string imgStr = string.Empty;

                var src = enableCrop ? item.GetCropUrl(cropPropAlias, cropName) : item.GetPropertyValue<string>("umbracoFile");

                imgTag.MergeAttribute("alt", item.Name + "-" + item.Id, false);
                imgTag.MergeAttribute("class", htmlClassName);

                if (isResponsive && item.HasValue("mobileImage"))
                {
                    imgTag.MergeAttribute(
                        "data-interchange", 
                        "[" + item.GetPropertyValue<string>("mobileImage") + ", (default)], ["
                        + src + ", (medium)]", 
                        false);

                    imgStr = imgTag + "<noscript><img src=\"" + src
                             + "\" alt=\"" + item.Name + "\" /></noscript>";
                }
                else
                {
                    imgTag.MergeAttribute("src", src, false);
                    imgStr = imgTag.ToString();
                }

                if (enableLink && item.GetPropertyValue<MultiUrls>("imageLink").Any())
                {
                    Link link = item.GetPropertyValue<MultiUrls>("imageLink").First();
                    var a = new TagBuilder("a");
                    a.MergeAttribute("href", link.Url, false);
                    a.MergeAttribute("target", link.Target, false);

                    a.InnerHtml = imgStr;

                    tags += a;
                }
                else
                {
                    tags += imgStr;
                }
            }

            return MvcHtmlString.Create(string.Join(" ", tags));
        }

        /// <summary>
        /// The images nodes for.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="maxItems">
        /// The max items.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<IPublishedContent> ImagesNodesFor(
            this IPublishedContent publishedContent, 
            string propertyAlias, 
            int maxItems = 1000)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return Enumerable.Empty<IPublishedContent>().ToList();
            }

            var helper = new UmbracoHelper(UmbracoContext.Current);

            IEnumerable<int> itemList =
                publishedContent.GetPropertyValue<string>(propertyAlias)
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);

            List<IPublishedContent> itemCollection = itemList.Select(helper.TypedMedia).Take(maxItems).ToList();

            return itemCollection.Any() ? itemCollection : Enumerable.Empty<IPublishedContent>().ToList();
        }

        /// <summary>
        /// The mntp count.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias. instance of Multinode Treepicker or Multi Media Picker
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int MntpCount(this IPublishedContent publishedContent, string propertyAlias)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return default(int);
            }

            return
                publishedContent.GetPropertyValue<string>(propertyAlias)
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Length;
        }

        /// <summary>
        /// The umb mntp nodes for.
        /// </summary>
        /// <param name="publishedContent">
        /// The published content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="maxItems">
        /// The max items.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<IPublishedContent> MntpNodesFor(
            this IPublishedContent publishedContent, 
            string propertyAlias, 
            int maxItems = 1000)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return Enumerable.Empty<IPublishedContent>().ToList();
            }

            var helper = new UmbracoHelper(UmbracoContext.Current);

            IEnumerable<int> mntpList =
                publishedContent.GetPropertyValue<string>(propertyAlias)
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);
            List<IPublishedContent> mntpCollection = mntpList.Select(helper.TypedContent).Where(x => x.IsVisible()).Take(maxItems).ToList();

            return mntpCollection.Any() ? mntpCollection : Enumerable.Empty<IPublishedContent>().ToList();
        }

        #endregion
    }
}