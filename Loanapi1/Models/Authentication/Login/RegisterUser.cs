

using Microsoft.AspNetCore.Identity;

namespace Loanapi1.Models.Authentication.Login
{
    public class RegisterUser
    {
        public  string? Username { get; set; }
        public string? Email { get; set; }
        public string?  Password { get; set; }
    }
    
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
