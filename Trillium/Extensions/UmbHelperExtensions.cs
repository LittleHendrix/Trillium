// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UmbHelperExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The umb helper extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The umb helper extensions.
    /// </summary>
    public static class UmbHelperExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The umb images count.
        /// </summary>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        /// <param name="publishedContent">
        /// TypeOf: IPublishedContent - e.g. Model.content or Umbraco.AssignedContentItem.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int UmbImagesCount(
            this UmbracoHelper umbracoHelper,
            IPublishedContent publishedContent,
            string propertyAlias)
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


        public static int UmbMntpCount(
            this UmbracoHelper umbracoHelper,
            IPublishedContent publishedContent,
            string propertyAlias)
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
        /// The umb images for.
        /// </summary>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        /// <param name="publishedContent">
        /// TypeOf: IPublishedContent - e.g. Model.content or Umbraco.AssignedContentItem.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="maxItems">
        /// Max output items - for return number of items in a multiPicker instance. Put 1 or leave empty for a singlePicker instance.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString UmbImagesFor(
            this UmbracoHelper umbracoHelper,
            IPublishedContent publishedContent,
            string propertyAlias,
            int maxItems = 1000)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return MvcHtmlString.Empty;
            }

            var umbItemsList = publishedContent.GetPropertyValue<string>(propertyAlias)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var umbItemsCollection = umbItemsList.Select(umbracoHelper.TypedMedia).Take(maxItems).ToList();

            if (!umbItemsCollection.Any())
            {
                return MvcHtmlString.Empty;
            }

            var tags = new List<TagBuilder>();

            foreach (var item in umbItemsCollection)
            {
                var imgTag = new TagBuilder("img");
                imgTag.Attributes.Add("src", item.GetPropertyValue<string>("umbracoFile"));
                imgTag.Attributes.Add("alt", item.Name + "-" + item.Id);

                tags.Add(imgTag);
            }

            return MvcHtmlString.Create(string.Join(" ", tags));
        }

        /// <summary>
        /// The umb images nodes for.
        /// </summary>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        /// <param name="publishedContent">
        /// TypeOf: IPublishedContent - e.g. Model.content or Umbraco.AssignedContentItem.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// <param name="maxItems">
        /// Max output items - for return number of items in a multiPicker instance. Put 1 or leave empty for a singlePicker instance.
        /// </param>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<IPublishedContent> UmbImagesNodesFor(
            this UmbracoHelper umbracoHelper,
            IPublishedContent publishedContent,
            string propertyAlias,
            int maxItems = 1000)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return Enumerable.Empty<IPublishedContent>().ToList();
            }

            var umbItemsList = publishedContent.GetPropertyValue<string>(propertyAlias)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

            var umbItemsCollection = umbItemsList.Select(umbracoHelper.TypedMedia).Take(maxItems).ToList();

            return umbItemsCollection.Any() ? umbItemsCollection : Enumerable.Empty<IPublishedContent>().ToList();
        }

        /// <summary>
        /// The umb images nodes for.
        /// </summary>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        /// <param name="publishedContent">
        /// TypeOf: IPublishedContent - e.g. Model.content or Umbraco.AssignedContentItem.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// <param name="maxItems">
        /// Max output items - for return number of items in a multiPicker instance. Put 1 or leave empty for a singlePicker instance.
        /// </param>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<IPublishedContent> UmbMntpNodesFor(
            this UmbracoHelper umbracoHelper,
            IPublishedContent publishedContent,
            string propertyAlias,
            int maxItems = 1000)
        {
            if (!publishedContent.HasValue(propertyAlias))
            {
                return Enumerable.Empty<IPublishedContent>().ToList();
            }

            var umbMntpList = publishedContent.GetPropertyValue<string>(propertyAlias)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var umbMntpCollection = umbMntpList.Select(umbracoHelper.TypedContent).Take(maxItems).ToList();

            return umbMntpCollection.Any() ? umbMntpCollection : Enumerable.Empty<IPublishedContent>().ToList();
        }

        #endregion
    }
}