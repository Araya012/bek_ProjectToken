using System.ComponentModel.DataAnnotations;
namespace bek_ProjectToken.Models
{
    public class Session
    {
        [Key]
        // Primary Key
        public int SessionId { get; set; }

        [Required(ErrorMessage = "ClientId is required")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "SessionToken is required")]
        public string SessionToken { get; set; }

        [Required(ErrorMessage = "CreationDate is required")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "ExpirationDate is required")]
        public DateTime ExpirationDate { get; set; }

        public Client Client { get; set; }
    }

}
