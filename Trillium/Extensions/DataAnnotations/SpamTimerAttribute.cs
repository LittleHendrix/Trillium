namespace Trillium.Extensions.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SpamTimerAttribute : ValidationAttribute
    {
        public SpamTimerAttribute(int timespan)
        {
            Timespan = timespan;
        }

        public int Timespan { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            long timestamp;

            if (long.TryParse(Convert.ToString(value), out timestamp))
            {
                var currentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                if (currentTime <= timestamp - Timespan)
                {
                    return
                        new ValidationResult(
                            string.Format(
                                "Invalid form submission. At least {0} seconds have to pass before form submission ({1})",
                                Timespan,
                                value));
                }
            }

            return ValidationResult.Success;
        }
    }
}