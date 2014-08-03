namespace Trillium.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContactViewModel
    {
        public string Captcha { get; set; }

        [Required]
        public DateTime SubmitDate { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}