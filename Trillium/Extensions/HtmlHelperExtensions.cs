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

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Truncate(this HtmlHelper html, string value, int count, string elipses)
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

        public static MvcHtmlString CustomTextAreaFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;

            var stringLength =
                member.Member.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as
                    StringLengthAttribute;

            var attributes = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);

            if (stringLength != null)
            {
                attributes.Add("maxlength", stringLength.MaximumLength);
            }

            return htmlHelper.TextAreaFor(expression, attributes);
        }
    }
}