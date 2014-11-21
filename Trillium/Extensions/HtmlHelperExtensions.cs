// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlHelperExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The html helper extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trillium.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using umbraco;

    /// <summary>
    ///     The html helper extensions.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The custom text area for.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString CustomTextAreaFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return CustomTextAreaFor(htmlHelper, expression, null);
        }

        /// <summary>
        ///     The custom text area for.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="htmlAttributes">
        ///     The html attributes.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString CustomTextAreaFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;

            var stringLength =
                member.Member.GetCustomAttributes(typeof (StringLengthAttribute), false).FirstOrDefault() as
                    StringLengthAttribute;

            var attributes = (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes);

            if (stringLength != null)
            {
                attributes.Add("maxlength", stringLength.MaximumLength);
            }

            return htmlHelper.TextAreaFor(expression, attributes);
        }

        /// <summary>
        ///     The custom text box for.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString CustomTextBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return CustomTextBoxFor(htmlHelper, expression, null);
        }

        /// <summary>
        ///     The custom text box for.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="htmlAttributes">
        ///     The html attributes.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString CustomTextBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;

            var stringLength =
                member.Member.GetCustomAttributes(typeof (StringLengthAttribute), false).FirstOrDefault() as
                    StringLengthAttribute;

            var attributes = (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes);

            if (stringLength != null)
            {
                attributes.Add("maxlength", stringLength.MaximumLength);
            }

            return htmlHelper.TextBoxFor(expression, attributes);
        }

        /// <summary>
        ///     The spam protection time stamp for.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString SpamProtectionTimeStampFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return SpamProtectionTimeStampFor(htmlHelper, expression, null);
        }

        /// <summary>
        ///     The spam protection time stamp.
        /// </summary>
        /// <param name="htmlHelper">
        ///     The html helper.
        /// </param>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="htmlAttributes">
        ///     The html attributes.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString SpamProtectionTimeStampFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            string propertyName = ExpressionHelper.GetExpressionText(expression);
            var timestamp = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var input = new TagBuilder("input");
            input.MergeAttribute("name",
                htmlHelper.AttributeEncode(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName)));
            input.MergeAttribute("value", timestamp.ToString());
            input.MergeAttribute("type", "hidden");
            input.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(input.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        ///     The truncate.
        /// </summary>
        /// <param name="html">
        ///     The html.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="count">
        ///     The count.
        /// </param>
        /// <param name="elipses">
        ///     The elipses.
        /// </param>
        /// <returns>
        ///     The <see cref="MvcHtmlString" />.
        /// </returns>
        public static MvcHtmlString Truncate(this HtmlHelper html, string value, int count = 500, string elipses = "...")
        {
            if (string.IsNullOrEmpty(value))
            {
                return MvcHtmlString.Empty;
            }

            string cleanHtml = library.StripHtml(value);

            return cleanHtml.Length <= count
                ? MvcHtmlString.Create(cleanHtml)
                : MvcHtmlString.Create(cleanHtml.Substring(0, count - 1) + elipses);
        }

        #endregion
    }
}