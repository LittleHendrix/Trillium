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
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    ///     The umb helper extensions.
    /// </summary>
    public static class UmbHelperExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The umb get typed node by alias.
        /// </summary>
        /// <param name="umbracoHelper">
        ///     The umbraco helper.
        /// </param>
        /// <param name="docTypeAlias">
        ///     The doc type alias.
        /// </param>
        /// <returns>
        ///     The <see cref="IPublishedContent" />. Please check for null ref before calling GetProperty()
        /// </returns>
        public static IPublishedContent UmbGetTypedNodeByAlias(this UmbracoHelper umbracoHelper, string docTypeAlias)
        {
            IEnumerable<IPublishedContent> root = umbracoHelper.TypedContentAtRoot();

            return root.DescendantsOrSelf(docTypeAlias).FirstOrDefault();
        }

        #endregion
    }
}