using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loanapi1.Models.Authentication.Login;
using Microsoft.AspNetCore.Identity;
using Loanapi1.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Loanapi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        
        private readonly UserManager<IdentityUser> usermanager;
        
        private readonly IConfiguration configuration;

        public AuthenticationController(Loanscontext loancontext,UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            
            this.usermanager = userManager;
            this.configuration = configuration;
                
        }
        [HttpPost]
        public async Task<IActionResult>Register([FromBody]RegisterUser registerUser , string role)
        {

            //try
            //{
                var userExist = await usermanager.FindByEmailAsync(registerUser.Email);
                if (userExist != null)
                {


                    return StatusCode(StatusCodes.Status403Forbidden,
                        new Response { Status = "Error", Message = "User Exists" });
                }
            //}
            //catch (Exception ex) {
            //    Console.WriteLine($"An error occurred: {ex.Message}");
            //    return StatusCode(StatusCodes.Status500InternalServerError,
            //        new Response { Status = "Error", Message = "An internal server error occurred" });
            //}



            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username

            };
            var result = await usermanager.CreateAsync(user,registerUser.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Failed to create " });
            }
            
           
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User failed to create2" });
            }
            
            

        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]Login login)
        {
            var user = await usermanager.FindByNameAsync(login.Username);

            if (user != null && await usermanager.CheckPasswordAsync(user, login.Password)) {
                var authclaims = new List<Claim>{
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var userroles = await usermanager.GetRolesAsync(user);
                foreach(var role in userroles)
                {
                    authclaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwttokens = GetToken(authclaims);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwttokens),
                    expiration = jwttokens.ValidTo
                });
            }

            
            return Unauthorized();
        }
        private JwtSecurityToken GetToken(List<Claim> authclaims)
        {
            var authsigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authclaims,
                signingCredentials: new SigningCredentials(authsigningKey, SecurityAlgorithms.HmacSha256));
            return token;

        }

    }
}
