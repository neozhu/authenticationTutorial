using System;
namespace basic.Models
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsPersistent { get; set; } = true;
    }

    public class  RegisterViewModel
    {
          public string Email { get; set; }
    public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
