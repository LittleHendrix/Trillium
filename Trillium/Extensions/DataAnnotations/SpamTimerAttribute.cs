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

                if (currentTime <= timestamp + Timespan)
                {
                    return
                        new ValidationResult(
                            string.Format(
                                "Your previous submission ({0}) is being processed. Please wait for {1} seconds before submitting the form again.",
                                value,
                                Timespan));
                }
            }

            return ValidationResult.Success;
        }
    }
}