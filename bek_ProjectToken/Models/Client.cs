using System.ComponentModel.DataAnnotations;

namespace bek_ProjectToken.Models
{
    public class Client
    {
        [Key]
        //Primary key
        public int ClientId { get; set; }

        [Required(ErrorMessage = "ClientKey is required")]

        //Identifies the client
        public string ClientKey { get; set; }

    }

}
