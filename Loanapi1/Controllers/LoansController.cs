using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loanapi1.Models;
using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace Loanapi1.Controllers
{
    
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class LoansController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Loanscontext _context;

        public LoansController(Loanscontext context)
        {
            _context = context;
            _connectionString = "server=DESKTOP-SBIR4S7\\SQLEXPRESS;database=loansDB;Trusted_Connection=True;Integrated Security=True;Encrypt=False;";
        }
        
        [HttpGet("checkstatus/{id}")]
        public IActionResult GetLoanApplicationById(int id)
        {
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT ApplicationID, Name, Amount, Loantype, Loanstatus FROM loanapplications WHERE ApplicationID = @ApplicationID";
                using (var command = new SqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@ApplicationID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        
                        if (reader.Read())
                        {
                            var loanApplication = new PendingLoanapplicationDto
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                Name = reader["Name"].ToString(),
                                Amount = Convert.ToInt32(reader["Amount"]),
                                Loanstatus = reader["Loanstatus"].ToString(),
                                Loantype = (loantypes)Enum.Parse(typeof(loantypes), reader["Loantype"].ToString(), true)
                            };

                            return Ok(loanApplication);
                        }
                        else
                        {
                            return NotFound(); 
                        }
                    }
                }
            }
        }

        //[HttpGet("All")]
        //public ActionResult<IEnumerable<Loanapplication>> GetLoanapplications(int page = 1, int pageSize = 10)
        //{
        //    var loanApplications = new List<Loanapplication>();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var query = "SELECT ApplicationID, Name, Amount, Loantype FROM loanapplications " +
        //                    "ORDER BY ApplicationID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        //            command.Parameters.AddWithValue("@PageSize", pageSize);

        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var loanApplication = new Loanapplication
        //                    {
        //                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
        //                        Name = reader["Name"].ToString(),
        //                        Amount = Convert.ToInt32(reader["Amount"]),
        //                        Loantype = Enum.Parse<loantypes>(reader["Loantype"].ToString()),
        //                    };

        //                    loanApplications.Add(loanApplication);
        //                }
        //            }
        //        }
        //    }

        //    return Ok(loanApplications);
        //}


        [HttpPost]
        public async Task<ActionResult> PostLoanapplication([FromBody] Loanapplication loanApplication)
        {

            if (string.IsNullOrEmpty(loanApplication.Loanstatus))
            {
                loanApplication.Loanstatus = "Pending";
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO loanapplications (Name, Amount, Loantype, Loanstatus)
                      VALUES (@Name, @Amount, @Loantype, @Loanstatus)";

                using (var command = new SqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@Name", loanApplication.Name);
                    command.Parameters.AddWithValue("@Amount", loanApplication.Amount);
                    command.Parameters.AddWithValue("@Loantype", loanApplication.Loantype);
                    command.Parameters.AddWithValue("@Loanstatus", loanApplication.Loanstatus);


                    await command.ExecuteNonQueryAsync();
                }
            }
            
            

            return CreatedAtAction("GetLoanapplications", loanApplication);
        }

        
       
        [HttpGet("/loanstate")]
        public IActionResult GetApprovedLoanApplications(string loanstate)
        {
            var pendingLoanApplications = new List<PendingLoanapplicationDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ApplicationID, Name, Amount, Loantype, Loanstatus FROM loanapplications WHERE Loanstatus = '" + loanstate + "'";

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
                                Loantype = (loantypes)Enum.Parse(typeof(loantypes), reader["Loantype"].ToString(), true),
                            };

                            pendingLoanApplications.Add(loanApplication);
                        }
                    }
                }
            }

            return Ok(pendingLoanApplications);
        }

        [HttpPut("loanapplications/{id}")]
        public IActionResult UpdateLoanStatus(int id, [FromBody] string newLoanStatus)
        {

            if (string.IsNullOrEmpty(newLoanStatus) || (newLoanStatus != "Approved" && newLoanStatus != "Rejected"))
            {
                return BadRequest("Invalid loan status. Please provide 'Approved' or 'Rejected'.");
            }


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var updateQuery = "UPDATE loanapplications SET Loanstatus = @Loanstatus WHERE ApplicationID = @ApplicationID";
                using (var command = new SqlCommand(updateQuery, connection))
                {

                    command.Parameters.AddWithValue("@Loanstatus", newLoanStatus);
                    command.Parameters.AddWithValue("@ApplicationID", id);


                    int rowsAffected = command.ExecuteNonQuery();


                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return Ok();
        }

    }

}

