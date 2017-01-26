namespace Auth.Cookies.AzureAD.Infrastructure
{
    public class AzureActiveDirectoryConfiguration
    {
        public string AadInstance { get; set; }

        public string ClientId { get; set; }

        public string Tenant { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public string Authority => string.Format(AadInstance, Tenant);
    }
}
