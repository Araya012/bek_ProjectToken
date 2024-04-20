using System.ComponentModel.DataAnnotations;

namespace bek_ProjectToken.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "ClientKey is required")]
        public string ClientKey { get; set; }

    }

}
