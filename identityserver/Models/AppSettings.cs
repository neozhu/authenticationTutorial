using System;
namespace identityserver.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            
    }
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audiance { get; set; }
    }
}
