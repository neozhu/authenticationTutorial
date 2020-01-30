using System;
namespace basic.App_Start
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
