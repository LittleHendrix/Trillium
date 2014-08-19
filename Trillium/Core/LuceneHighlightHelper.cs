// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuceneHighlightHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The lucene highlight helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Core
{
    using System.Collections.Generic;
    using System.IO;
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Highlight;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Util;

    /// <summary>
    ///     The lucene highlight helper.
    /// </summary>
    public class LuceneHighlightHelper
    {
        #region Static Fields

        /// <summary>
        ///     The lucene instance.
        /// </summary>
        private static readonly LuceneHighlightHelper LuceneInstance = new LuceneHighlightHelper();

        #endregion

        #region Fields

        /// <summary>
        ///     The lucene version.
        /// </summary>
        private readonly Version luceneVersion = Version.LUCENE_29;

        /// <summary>
        ///     The query parsers.
        /// </summary>
        private readonly Dictionary<string, QueryParser> queryParsers = new Dictionary<string, QueryParser>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="LuceneHighlightHelper" /> class from being created.
        /// </summary>
        private LuceneHighlightHelper()
        {
            Separator = "...";
            MaxNumHighlights = 3;
            HighlightAnalyzer = new StandardAnalyzer(luceneVersion);
            HighlightFormatter = new SimpleHTMLFormatter(string.Empty, " ");
        }

        #endregion

        #region Public Properties

        public static LuceneHighlightHelper Instance
        {
            get { return LuceneInstance; }
        }

        /// <summary>
        ///     Gets or sets the highlight analyzer.
        /// </summary>
        public Analyzer HighlightAnalyzer { get; set; }

        /// <summary>
        ///     Gets or sets the highlight formatter.
        /// </summary>
        public Formatter HighlightFormatter { get; set; }

        /// <summary>
        ///     Gets or sets the max num highlights.
        /// </summary>
        public int MaxNumHighlights { get; set; }

        /// <summary>
        ///     Gets or sets the separator.
        /// </summary>
        public string Separator { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get highlight.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="highlightField">
        ///     The highlight field.
        /// </param>
        /// <param name="searcher">
        ///     The searcher.
        /// </param>
        /// <param name="luceneRawQuery">
        ///     The lucene raw query.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetHighlight(string value, string highlightField, Searcher searcher, string luceneRawQuery)
        {
            Query query = GetQueryParser(highlightField).Parse(luceneRawQuery);
            var scorer = new QueryScorer(searcher.Rewrite(query));

            var highlighter = new Highlighter(HighlightFormatter, scorer);

            TokenStream tokenStream = HighlightAnalyzer.TokenStream(highlightField, new StringReader(value));
            return highlighter.GetBestFragments(tokenStream, value, MaxNumHighlights, Separator);
        }

        /// <summary>
        ///     The get highlight.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="highlightField">
        ///     The highlight field.
        /// </param>
        /// <param name="searcher">
        ///     The searcher.
        /// </param>
        /// <param name="luceneRawQuery">
        ///     The lucene raw query.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetHighlight(string value, string highlightField, IndexSearcher searcher, string luceneRawQuery)
        {
            Query query = GetQueryParser(highlightField).Parse(luceneRawQuery);
            var scorer = new QueryScorer(query.Rewrite(searcher.GetIndexReader()));

            var highlighter = new Highlighter(HighlightFormatter, scorer);

            TokenStream tokenStream = HighlightAnalyzer.TokenStream(highlightField, new StringReader(value));
            return highlighter.GetBestFragments(tokenStream, value, MaxNumHighlights, Separator);
        }

        /// <summary>
        ///     The get highlight.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="searcher">
        ///     The searcher.
        /// </param>
        /// <param name="highlightField">
        ///     The highlight field.
        /// </param>
        /// <param name="luceneQuery">
        ///     The lucene query.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetHighlight(string value, IndexSearcher searcher, string highlightField, Query luceneQuery)
        {
            var scorer = new QueryScorer(luceneQuery.Rewrite(searcher.GetIndexReader()));
            var highlighter = new Highlighter(HighlightFormatter, scorer);

            TokenStream tokenStream = HighlightAnalyzer.TokenStream(highlightField, new StringReader(value));
            return highlighter.GetBestFragments(tokenStream, value, MaxNumHighlights, Separator);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get query parser.
        /// </summary>
        /// <param name="highlightField">
        ///     The highlight field.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParser" />.
        /// </returns>
        protected QueryParser GetQueryParser(string highlightField)
        {
            if (!queryParsers.ContainsKey(highlightField))
            {
                queryParsers[highlightField] = new QueryParser(
                    luceneVersion,
                    highlightField,
                    HighlightAnalyzer);
            }

            return queryParsers[highlightField];
        }

        #endregion
    }
}