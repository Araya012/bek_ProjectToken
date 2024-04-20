using System.ComponentModel.DataAnnotations;

namespace bek_ProjectToken.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Name cannot contain numbers")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

    }
}

