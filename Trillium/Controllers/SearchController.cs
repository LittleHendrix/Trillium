// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchController.cs" company="">
//   
// </copyright>
// <summary>
//   The search controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Controllers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Examine;
    using Examine.LuceneEngine;
    using Examine.Providers;
    using Examine.SearchCriteria;
    using Lucene.Net.Search;
    using Trillium.Core;
    using Trillium.ViewModels;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    ///     The search controller.
    /// </summary>
    public class SearchController : RenderMvcController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The index.
        /// </summary>
        /// <param name="umbracoModel">
        ///     The umbraco model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public override ActionResult Index(RenderModel umbracoModel)
        {
            return GetDefaultAction(umbracoModel);
        }

        /// <summary>
        ///     The search.
        /// </summary>
        /// <param name="umbracoModel">
        ///     The umbraco model.
        /// </param>
        /// <param name="searchTerms">
        ///     The search terms.
        /// </param>
        /// <param name="json">
        ///     The json.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Search(RenderModel umbracoModel, string searchTerms = "", string json = "")
        {
            if (!string.IsNullOrEmpty(json))
            {
                var jsonResults =
                    GetSearchResults(searchTerms, 100, false)
                        .Results.Select(x => new {Url = Umbraco.NiceUrl(x.Id), Title = x.Fields["nodeName"]});

                return Json(jsonResults);
            }

            SearchResultsViewModel model = GetSearchResults(searchTerms, 20, true);

            return GetDefaultAction(model);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get default action.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        private ActionResult GetDefaultAction(object model)
        {
            // get the template name from the route values:
            // string template = this.ControllerContext.RouteData.Values["action"].ToString();

            // return this.View(template, model);
            return CurrentTemplate(model);
        }

        /// <summary>
        ///     The get search results.
        /// </summary>
        /// <param name="searchTerms">
        ///     The search terms.
        /// </param>
        /// <param name="pageSize">
        ///     The page size.
        /// </param>
        /// <param name="applyHighlighting">
        ///     The apply Highlighting.
        /// </param>
        /// <returns>
        ///     The <see cref="SearchResultsViewModel" />.
        /// </returns>
        private SearchResultsViewModel GetSearchResults(string searchTerms, int pageSize, bool applyHighlighting)
        {
            int page;
            int resultCount = 0;
            int totalPages = 0;
            IOrderedEnumerable<SearchResult> searchResults = null;
            Searcher luceneSearcher = null;

            if (!int.TryParse(Request.QueryString["Page"], out page))
            {
                page = 1;
            }

            if (!string.IsNullOrEmpty(searchTerms))
            {
                searchTerms = searchTerms.ToLower();
                BaseSearchProvider searchProvider = ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
                ISearchCriteria searchCriteria = searchProvider.CreateSearchCriteria(BooleanOperation.Or);
                IBooleanOperation queryBuilder = null;

                foreach (string searchValue in
                    searchTerms.Split(' ').Where(searchTerm => !string.IsNullOrEmpty(searchTerm.RemoveStopWords())))
                {
                    if (queryBuilder == null)
                    {
                        queryBuilder =
                            searchCriteria.GroupedOr(
                                new[] {"nodeName", "metaTitle", "metaDescription", "pageHeading", "bodyText"},
                                searchValue);
                    }
                    else
                    {
                        queryBuilder =
                            queryBuilder.Or()
                                .GroupedOr(
                                    new[] {"nodeName", "metaTitle", "metaDescription", "pageHeading", "bodyText"},
                                    searchValue);
                    }
                }

                if (queryBuilder != null)
                {
                    queryBuilder = queryBuilder.Not().Field("template", "0");

                    ISearchCriteria query =
                        queryBuilder.Not()
                            .GroupedOr(new[] {"umbracoNaviHide", "nodeTypeAlias"}, new[] {"1", "Folder"})
                            .Compile();

                    ISearchResults results = searchProvider.Search(query);
                    luceneSearcher = ((SearchResults) results).LuceneSearcher;
                    searchResults = results.OrderByDescending(x => x.Score);
                    resultCount = results.TotalItemCount;
                }

                totalPages = (int) Math.Ceiling(resultCount/(double) pageSize);

                if (page > totalPages)
                {
                    page = totalPages;
                }
                else if (page < 1)
                {
                    page = 1;
                }

                if (applyHighlighting && searchResults != null)
                {
                    foreach (SearchResult item in searchResults)
                    {
                        const string FieldName = "bodyText";
                        string searchHighlight = string.Empty;
                        string searchTitle = item.Fields["nodeName"];

                        if (item.Fields.ContainsKey(FieldName))
                        {
                            string fieldValue = item.Fields[FieldName];
                            searchHighlight = LuceneHighlightHelper.Instance.GetHighlight(
                                fieldValue,
                                FieldName,
                                luceneSearcher,
                                searchTerms);

                            if (string.IsNullOrEmpty(searchHighlight))
                            {
                                if (!string.IsNullOrEmpty(fieldValue))
                                {
                                    searchHighlight = Umbraco.Truncate(fieldValue, 200).ToString();
                                }
                                else if (item.Fields.ContainsKey("metaDescription"))
                                {
                                    searchHighlight =
                                        Umbraco.Truncate(item.Fields["metaDescription"], 200).ToString();
                                }
                            }
                        }
                        else if (item.Fields.ContainsKey("metaDescription"))
                        {
                            searchHighlight =
                                Umbraco.Truncate(item.Fields["metaDescription"], 200).ToString();
                        }

                        foreach (
                            string searchTerm in
                                searchTerms.Split(' ')
                                    .Where(searchTerm => !string.IsNullOrEmpty(searchTerm.RemoveStopWords())))
                        {
                            searchHighlight = HighlightText(searchHighlight, searchTerm);
                            searchTitle = HighlightText(searchTitle, searchTerm);
                        }

                        item.Fields.Add("searchHighlight", searchHighlight);
                        item.Fields.Add("searchTitle", searchTitle);
                    }
                }
            }

            var searchResultModel = new SearchResultsViewModel
            {
                Page = page,
                PageSize = pageSize,
                ResultCount = resultCount,
                TotalPages = totalPages,
                SearchTerms = searchTerms
            };

            if (searchResults != null)
            {
                searchResultModel.Results = searchResults.Skip((page - 1)*pageSize).Take(pageSize);
            }

            searchResultModel.SearchTerms = searchTerms;

            return searchResultModel;
        }

        private static string HighlightText(string searchHighlight, string searchTerm)
        {
            searchHighlight = Regex.Replace(searchHighlight, searchTerm, "<strong>$&</strong>", RegexOptions.IgnoreCase);
            return searchHighlight;
        }

        #endregion
    }
}