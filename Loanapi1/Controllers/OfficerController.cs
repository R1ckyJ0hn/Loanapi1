using Loanapi1.Models;
using Loanapi1.Models.Authentication.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loanapi1.Controllers
{
    
    [Route("officer")]
    [ApiController]
    public class OfficerController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Loanscontext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public OfficerController(Loanscontext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _connectionString = "Data Source=DESKTOP-NE6M66C;Initial Catalog=LoanUsers1;user id =sa;password=P@ssw0rd;Trusted_Connection=True;Integrated Security=True;Encrypt=False;";
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        [Authorize(Roles = UserRoles.Loanofficer)]
        [HttpGet]
        public ActionResult<IEnumerable<Loanapplication>> GetLoanapplications(int page = 1, int pageSize = 10)
        {
            var loanApplications = new List<Loanapplication>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ApplicationID, Name, Amount, Loantype FROM Loans " +
                            "ORDER BY ApplicationID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loanApplication = new Loanapplication
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                Name = reader["Name"].ToString(),
                                Amount = Convert.ToInt32(reader["Amount"]),
                                Loantype = reader["Loantype"].ToString(),
                            };

                            loanApplications.Add(loanApplication);
                        }
                    }
                }
            }

            return Ok(loanApplications);
        }
        [Authorize(Roles = UserRoles.Loanofficer)]
        [HttpGet("status")]
        public IActionResult GetApprovedLoanApplications(string loanstate)
        {
            var pendingLoanApplications = new List<PendingLoanapplicationDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ApplicationID, Name, Amount, Loantype, Loanstatus FROM Loans WHERE Loanstatus = '" + loanstate + "'";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loanApplication = new PendingLoanapplicationDto
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                Name = reader["Name"].ToString(),
                                Amount = Convert.ToInt32(reader["Amount"]),
                                Loanstatus = reader["Loanstatus"].ToString(),
                                Loantype =  reader["Loantype"].ToString(),
                            };

                            pendingLoanApplications.Add(loanApplication);
                        }
                    }
                }
            }

            return Ok(pendingLoanApplications);
        }
        [Authorize(Roles = UserRoles.Loanofficer)]
        [HttpPut("updatestatus")]
        public IActionResult NewLoanStatus(int id, [FromBody] string newLoanStatus)
        {

            if (string.IsNullOrEmpty(newLoanStatus) || (newLoanStatus != "Approved" && newLoanStatus != "Rejected"))
            {
                Console.WriteLine("Invalid Status provided");
                return BadRequest("Invalid loan status. Please provide 'Approved' or 'Rejected'.");

                
            }


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var updateQuery = "UPDATE Loans SET Loanstatus = @Loanstatus WHERE ApplicationID = @ApplicationID";
                using (var command = new SqlCommand(updateQuery, connection))
                {
                    

                        command.Parameters.AddWithValue("@Loanstatus", newLoanStatus);
                        command.Parameters.AddWithValue("@ApplicationID", id);



                        int rowsAffected = command.ExecuteNonQuery();


                        try
                        {
                            if (rowsAffected == 0)
                            {

                                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Application ID is not found" });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error has occured--Message:{ex.ToString()}");
                        }
    
                }
            }

            return Ok();
        }

    }


}

