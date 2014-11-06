namespace Trillium.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Trillium.Extensions.DataAnnotations;

    public class ContactViewModel
    {
        [SpamPot(ErrorMessage = "Honeypot must be left empty")]
        public string Honeypot { get; set; }

        [Required]
        public DateTime SubmitDate { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "Name cannot exceed 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "Email address cannot exceed 50 characters")]
        public string EmailAddress { get; set; }

        [StringLength(50, ErrorMessage = "Subject cannot exceed 50 characters")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1024, ErrorMessage = "Message cannot exceed 1024 characters")]
        public string Message { get; set; }
    }
}