using Loanapi1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Loanapi1.Controllers
{
    //[Authorize]
    [Route("officer")]
    [ApiController]
    public class OfficerController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Loanscontext _context;

        public OfficerController(Loanscontext context)
        {
            _context = context;
            _connectionString = "server=DESKTOP-SBIR4S7\\SQLEXPRESS;database=loansDB;Trusted_Connection=True;Integrated Security=True;Encrypt=False;";
        }

        [HttpGet("All")]
        public ActionResult<IEnumerable<Loanapplication>> GetLoanapplications(int page = 1, int pageSize = 10)
        {
            var loanApplications = new List<Loanapplication>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ApplicationID, Name, Amount, Loantype FROM Loanapplication " +
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
                                Loantype = Enum.Parse<loantypes>(reader["Loantype"].ToString()),
                            };

                            loanApplications.Add(loanApplication);
                        }
                    }
                }
            }

            return Ok(loanApplications);
        }

    }
}
