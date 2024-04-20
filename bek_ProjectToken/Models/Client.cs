using System.ComponentModel.DataAnnotations;

namespace bek_ProjectToken.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string? ClientKey { get; set; }
        // Other fields related to the client, if necessary
    }

}
