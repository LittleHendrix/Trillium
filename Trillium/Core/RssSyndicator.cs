﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssSyndicator.cs" company="">
//   
// </copyright>
// <summary>
//   The rss syndicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Web;
    using System.Web.Caching;
    using Trillium.Extensions;
    using umbraco;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    /// <summary>
    ///     The rss syndicator.
    /// </summary>
    public class RssSyndicator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get feed.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="SyndicationFeed" />.
        /// </returns>
        public static SyndicationFeed GetFeed(RenderModel model)
        {
            var cachedFeed = HttpContext.Current.Cache["cachedFeed"] as SyndicationFeed;
            var cachedRequest = HttpContext.Current.Cache["cachedRequest"] as Uri;
            //// return cachedFeed if not expired and incoming request is from the same url
            if (cachedFeed != null && cachedRequest == HttpContext.Current.Request.Url)
            {
                return cachedFeed;
            }

            var rssFeed = new SyndicationFeed();
            Uri rawUrl = HttpContext.Current.Request.Url;
            string pbaseUrl = string.Format("{0}://{1}", rawUrl.Scheme, rawUrl.Host);

            string feedTitle = model.Content.HasValue("feedTitle")
                ? model.Content.GetPropertyValue<string>("feedTitle")
                : model.Content.Name;
            string feedDescri = model.Content.HasValue("feedDescription")
                ? model.Content.GetPropertyValue<string>("feedDescription")
                : HttpContext.Current.Request.Url.Host;

            rssFeed.Id = pbaseUrl;
            rssFeed.Title = new TextSyndicationContent(feedTitle);
            rssFeed.Description = new TextSyndicationContent(feedDescri);
            rssFeed.Copyright = new TextSyndicationContent("Trillium Fitness");
            rssFeed.BaseUri = new Uri(pbaseUrl + rawUrl.AbsolutePath);
            rssFeed.LastUpdatedTime = new DateTimeOffset(DateTime.UtcNow);
            rssFeed.Language = "en-us";
            rssFeed.ImageUrl = new Uri(pbaseUrl + "/assets/images/rss_logo.png");

            var link = new SyndicationLink(new Uri(pbaseUrl + rawUrl.AbsolutePath + "/rss.xml"))
            {
                RelationshipType =
                    "self",
                MediaType =
                    "text/html",
                Title =
                    "Trillium Fitness - "
                    + model.Content
                        .Name
                    + " Feed"
            };

            rssFeed.Links.Add(link);

            link = new SyndicationLink(new Uri(pbaseUrl + rawUrl.AbsolutePath))
            {
                MediaType = "text/html",
                Title =
                    "Trillium Fitness - "
                    + model.Content.Name + " Feed"
            };

            rssFeed.Links.Add(link);

            rssFeed.Items = GetFeedItems(pbaseUrl, model);

            HttpContext.Current.Cache.Insert("cachedFeed", rssFeed, null, DateTime.UtcNow.AddMinutes(1),
                Cache.NoSlidingExpiration);
            HttpContext.Current.Cache.Insert("cachedRequest", rawUrl, null, Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(1));

            return rssFeed;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get feed items.
        /// </summary>
        /// <param name="pbaseUrl">
        ///     The pbase url.
        /// </param>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        private static IEnumerable<SyndicationItem> GetFeedItems(string pbaseUrl, IRenderModel model)
        {
            var items = new List<SyndicationItem>();

            foreach (IPublishedContent item in
                model.Content.Children(x => x.IsVisible()).OrderByDescending(x => x.UpdateDate))
            {
                string title = item.HasValue("pageHeading") ? item.GetPropertyValue<string>("pageHeading") : item.Name;
                DateTime pubDate = item.HasValue("publishDate")
                    ? item.GetPropertyValue<DateTime>("publishDate")
                    : item.CreateDate;

                // var summary = item.GetPropertyValue<string>("metaDescription");
                string content = item.HasValue("bodyText")
                    ? library.TruncateString(item.GetPropertyValue<string>("bodyText"), 250, "...")
                    : string.Empty;

                if (item.HasValue("pageMedia"))
                {
                    IPublishedContent img = item.ImagesNodesFor("pageMedia", 1).FirstOrDefault();
                    if (img != null) content += "<p><img src=\"" + img.Url + "\" alt=\"" + img.Name + "\" /></p>";
                }

                string id = item.UrlName + "-" + item.Id + "-" + item.CreateDate.ToString("u");
                var url = new Uri(item.UrlWithDomain());

                var feedItem = new SyndicationItem
                {
                    Title = new TextSyndicationContent(title),

                    // Summary = (!string.IsNullOrEmpty(summary) ? new TextSyndicationContent(summary) : null),
                    Content = new TextSyndicationContent(content),
                    BaseUri = url,
                    PublishDate = pubDate,
                    LastUpdatedTime = item.UpdateDate,
                    Id = id
                };

                feedItem.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(pbaseUrl + item.Url)));

                items.Add(feedItem);
            }

            return items;
        }

        #endregion
    }
}