namespace Trillium.Extensions.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class SpamPotAttribute : ValidationAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule {ErrorMessage = ErrorMessage, ValidationType = "spampot"};
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return string.IsNullOrEmpty(Convert.ToString(value))
                ? ValidationResult.Success
                : new ValidationResult("Honeypot must be left empty");
        }
    }
}