namespace Trillium.Controllers.CustomControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Trillium.ViewModels;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    public class BlogController : RenderMvcController
    {
        [OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Blog(RenderModel umbModel)
        {
            var page = this.HttpContext.Request.QueryString["Page"] ?? "1";
            var pageInt = Convert.ToInt32(page);
            var vm = new BlogViewModel
            {
                Page = pageInt
            };

            vm.BlogPost = GetPagedBlogPost(vm);

            return this.CurrentTemplate(vm);
        }

        private static IEnumerable<IPublishedContent> GetPagedBlogPost(BlogViewModel model)
        {
            var pageSise = model.Content.HasValue("itemsPerPage") ? Convert.ToInt32(model.Content.GetPropertyValue("itemsPerPage")) : model.PageSize;
            var skipItems = (pageSise * model.Page) - pageSise;

            var posts = model.Content.Children.Where(x => x.IsVisible()).OrderByDescending(x => x.HasValue("publishDate") ? x.GetPropertyValue<DateTime>("publishDate") : x.CreateDate).ToList();
            model.TotalPages = Convert.ToInt32(Math.Ceiling((double)posts.Count() / pageSise));

            model.PreviousPage = model.Page - 1;
            model.NextPage = model.Page + 1;

            model.IsFirstPage = model.Page <= 1;
            model.IsLastPage = model.Page >= model.TotalPages;

            return posts.Skip(skipItems).Take(pageSise);
        }
    }
}