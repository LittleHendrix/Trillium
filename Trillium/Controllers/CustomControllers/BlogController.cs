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
        private readonly BlogViewModel blogViewModel;

        public BlogController()
        {
            blogViewModel = new BlogViewModel();
        }

        [OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Blog(RenderModel umbModel)
        {
            string page = HttpContext.Request.QueryString["Page"] ?? "1";
            int pageInt = Convert.ToInt32(page);
            blogViewModel.Page = pageInt;
            blogViewModel.BlogPost = GetPagedBlogPost(blogViewModel);

            return CurrentTemplate(blogViewModel);
        }

        private static IEnumerable<IPublishedContent> GetPagedBlogPost(BlogViewModel model)
        {
            int pageSise = model.Content.HasValue("itemsPerPage")
                ? Convert.ToInt32(model.Content.GetPropertyValue("itemsPerPage"))
                : model.PageSize;
            int skipItems = (pageSise*model.Page) - pageSise;

            List<IPublishedContent> posts =
                model.Content.Children.Where(x => x.IsVisible())
                    .OrderByDescending(
                        x => x.HasValue("publishDate") ? x.GetPropertyValue<DateTime>("publishDate") : x.CreateDate)
                    .ToList();
            model.TotalPages = Convert.ToInt32(Math.Ceiling((double) posts.Count()/pageSise));

            model.PreviousPage = model.Page - 1;
            model.NextPage = model.Page + 1;

            model.IsFirstPage = model.Page <= 1;
            model.IsLastPage = model.Page >= model.TotalPages;

            return posts.Skip(skipItems).Take(pageSise);
        }
    }
}