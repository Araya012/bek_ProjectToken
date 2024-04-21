using System.ComponentModel.DataAnnotations;

namespace bek_ProjectToken.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The name field is required")]
        [MaxLength(50, ErrorMessage = "The name field cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Only letters, accents, and spaces are allowed")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email field is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
