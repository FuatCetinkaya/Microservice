namespace Web.Configuration
{
    public class ClientSettings
    {
        public Client WebMVCClient { get; set; }
        public Client WebMVCClientForUser { get; set; }
    }

    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecrets { get; set; }
    }
}
