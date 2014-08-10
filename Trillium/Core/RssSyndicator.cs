namespace Trillium.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Web;
    using umbraco;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    public class RssSyndicator
    {

        public static SyndicationFeed GetFeed(RenderModel model)
        {
            var rssFeed = new SyndicationFeed();
            var rawUrl = HttpContext.Current.Request.Url;
            var pbaseUrl = string.Format("{0}://{1}", rawUrl.Scheme, rawUrl.Host);

            var feedTitle = model.Content.HasValue("blogTitle") ? model.Content.GetPropertyValue<string>("blogTitle") : model.Content.Name;
            var feedDescri = model.Content.HasValue("blogDescription") ? model.Content.GetPropertyValue<string>("blogDescription") : HttpContext.Current.Request.Url.Host;

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
                RelationshipType = "self",
                MediaType = "text/html",
                Title = "Trillium Fitness - " + model.Content.Name + " Feed"
            };

            rssFeed.Links.Add(link);

            link = new SyndicationLink(new Uri(pbaseUrl + rawUrl.AbsolutePath))
            {
                MediaType = "text/html",
                Title = "Trillium Fitness - " + model.Content.Name + " Feed"
            };

            rssFeed.Links.Add(link);

            rssFeed.Items = GetFeedItems(pbaseUrl, model);

            return rssFeed;
        }

        private static IEnumerable<SyndicationItem> GetFeedItems(string pbaseUrl, IRenderModel model)
        {
            var items = new List<SyndicationItem>();
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            foreach (var item in model.Content.Children(x => x.IsVisible()).OrderByDescending(x => x.UpdateDate))
            {
                var title = item.HasValue("pageHeading") ? item.GetPropertyValue<string>("pageHeading") : item.Name;
                var pubDate = item.HasValue("publishDate") ? item.GetPropertyValue<DateTime>("publishDate") : item.CreateDate;
                //var summary = item.GetPropertyValue<string>("metaDescription");
                var content = item.HasValue("bodyText") ? library.TruncateString(item.GetPropertyValue<string>("bodyText"), 250, "...").ToString() : string.Empty;
               
                
                if (item.HasValue("pageMedia"))
                {
                    var img = umbracoHelper.TypedMedia(item.GetPropertyValue<int>("pageMedia"));
                    content += "<p><img src=\"" + img.Url + "\" alt=\"" + img.Name + "\" /></p>";
                }

                var id = item.UrlName + "-" + item.Id + "-" + item.CreateDate.ToString("u");
                var url = new Uri(item.UrlWithDomain());

                var feedItem = new SyndicationItem
                {
                    Title = new TextSyndicationContent(title),
                    //Summary = (!string.IsNullOrEmpty(summary) ? new TextSyndicationContent(summary) : null),
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
    }
}