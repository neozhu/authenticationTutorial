using System;
namespace basic.Models
{
    public class OAuthRequest
    {
        public OAuthRequest()
        {
        }
        public string Response_Type { get; set; } // authorization flow type 
        public string Client_Id { get; set; }// client id
        public string Redirect_Uri { get; set; }
        public string Scope { get; set; } // what info I want = email,grandma,tel
        public string State { get; set; }
    }

    public class OAuthLoginViewModel {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }
    }
    public class TokenRequest {

        public string Grant_Type { get; set; }
        public string Code { get; set; }
        public string Redirect_Uri { get; set; }
        public string Client_Id { get; set; }
        public string Refresh_Token { get; set; }
    }
}
