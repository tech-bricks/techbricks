using System.ComponentModel.DataAnnotations;

namespace TechBricks.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Please provide your name.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        [MinLength(10, ErrorMessage = "The message must be at least 10 characters long.")]
        public string Message { get; set; }
    }
}
