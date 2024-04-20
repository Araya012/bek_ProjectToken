namespace apiv2.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int ClientId { get; set; }
        public string? SessionToken { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        // Other fields related to the session, if necessary

        // Navigation property to establish the relationship with Client
        public Client? Client { get; set; }
    }

}
