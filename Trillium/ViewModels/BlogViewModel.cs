namespace Trillium.ViewModels
{
    using System.Collections.Generic;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    public class BlogViewModel : RenderModel
    {
        public BlogViewModel()
            : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent)
        {
            this.PageSize = 5;
        }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int PreviousPage { get; set; }

        public int NextPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public IEnumerable<IPublishedContent> BlogPost { get; set; }
    }
}