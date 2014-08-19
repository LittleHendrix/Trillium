// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The search results view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.ViewModels
{
    using System.Collections.Generic;
    using Examine;

    /// <summary>
    ///     The search results view model.
    /// </summary>
    public class SearchResultsViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchResultsViewModel" /> class.
        /// </summary>
        public SearchResultsViewModel()
        {
            Results = new List<SearchResult>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///     Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Gets or sets the result count.
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        ///     Gets or sets the results.
        /// </summary>
        public IEnumerable<SearchResult> Results { get; set; }

        /// <summary>
        ///     Gets or sets the search terms.
        /// </summary>
        public string SearchTerms { get; set; }

        /// <summary>
        ///     Gets or sets the total pages.
        /// </summary>
        public int TotalPages { get; set; }

        #endregion
    }
}