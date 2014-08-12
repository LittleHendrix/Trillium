namespace Trillium.Extensions
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public static class TagBuilderExtensions
    {
        public static void CustomMergeAttributes(this TagBuilder tagBuilder, IDictionary<string, object> attributes)
        {
            foreach (var attribute in attributes)
            {
                string currentValue;
                string newValue = attribute.Value.ToString();

                if (tagBuilder.Attributes.TryGetValue(attribute.Key, out currentValue))
                {
                    newValue = currentValue + " " + newValue;
                }

                tagBuilder.Attributes[attribute.Key] = newValue;
            }
        }
    }
}