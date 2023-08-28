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
using Loanapi1.Models.Authentication.Login;

namespace Loanapi1.Controllers
{


    [Route("user")]
    [ApiController]
    //[Authorize]


    public class UserController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Loanscontext _context;

        public UserController(Loanscontext context)
        {
            _context = context;
            _connectionString = "Data Source=DESKTOP-NE6M66C;Initial Catalog=LoanUsers1;user id =sa;password=P@ssw0rd;Trusted_Connection=True;Integrated Security=True;Encrypt=False;";
        }
        [HttpPost("newapplication")]
        public async Task<ActionResult> PostLoanapplication([FromBody] Loanapplication loanApplication)
        {
            if (string.IsNullOrEmpty(loanApplication.Loanstatus))
            {
                loanApplication.Loanstatus = "Pending";
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "select * from types where loantype=@loantype";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@loantype", loanApplication.Loantype);
                    var iffound = command.ExecuteScalar();
                    if (iffound!=null)
                    {
                        int createdApplicationId = 0;

                        using (var connection1 = new SqlConnection(_connectionString))
                        {
                            connection1.Open();
                            var query1 = @"INSERT INTO Loans (Name, Amount, Loantype, Loanstatus)
                                             VALUES (@Name, @Amount, @Loannewtype, @Loanstatus);
                                             SELECT SCOPE_IDENTITY();";

                            using (var command1 = new SqlCommand(query1, connection1))
                            {
                                command.Parameters.AddWithValue("@Name", loanApplication.Name);
                                command.Parameters.AddWithValue("@Amount", loanApplication.Amount);
                                command.Parameters.AddWithValue("@Loannewtype", loanApplication.Loantype);
                                command.Parameters.AddWithValue("@Loanstatus", loanApplication.Loanstatus);


                                createdApplicationId = Convert.ToInt32(await command.ExecuteScalarAsync());
                                
                            }
                        }

                        return Ok(new Response
                        {
                            Status = "Success",
                            Message = $"Applied and your application ID is {createdApplicationId}.Please do not share your applicationID"
                        });

                    }
                    return NotFound(new Response { Status="Error" ,Message= " Enter a valid loan type"}); 

                }

            }

            
        }


        [HttpGet("checkstatus")]
        public IActionResult GetLoanApplicationById(int id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT ApplicationID, Name, Amount, Loantype, Loanstatus FROM Loans WHERE ApplicationID = @ApplicationID";
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
                                Loantype = reader["Loantype"].ToString(),
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
        [HttpGet("loantypes")]
        public async Task<ActionResult<IEnumerable<Loantypes>>> GetLoantypes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM types";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var loantypesList = new List<Loantypes>();

                    while (await reader.ReadAsync())
                    {
                        var loantypes = new Loantypes
                        {
                            TypeId = reader.GetInt32(reader.GetOrdinal("TypeId")),
                            loantype = reader.GetString(reader.GetOrdinal("loantype"))
                        };

                        loantypesList.Add(loantypes);
                    }

                    return Ok(loantypesList);
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






        //[HttpGet("/loanstate")]
        //public IActionResult GetApprovedLoanApplications(string loanstate)
        //{
        //    var pendingLoanApplications = new List<PendingLoanapplicationDto>();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var query = "SELECT ApplicationID, Name, Amount, Loantype, Loanstatus FROM loanapplications WHERE Loanstatus = '" + loanstate + "'";

        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var loanApplication = new PendingLoanapplicationDto
        //                    {
        //                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
        //                        Name = reader["Name"].ToString(),
        //                        Amount = Convert.ToInt32(reader["Amount"]),
        //                        Loanstatus = reader["Loanstatus"].ToString(),
        //                        Loantype = (loantypes)Enum.Parse(typeof(loantypes), reader["Loantype"].ToString(), true),
        //                    };

        //                    pendingLoanApplications.Add(loanApplication);
        //                }
        //            }
        //        }
        //    }

        //    return Ok(pendingLoanApplications);
        //}

        //        [HttpPut("loanapplications/{id}")]
        //        public IActionResult UpdateLoanStatus(int id, [FromBody] string newLoanStatus)
        //        {

        //            if (string.IsNullOrEmpty(newLoanStatus) || (newLoanStatus != "Approved" && newLoanStatus != "Rejected"))
        //            {
        //                return BadRequest("Invalid loan status. Please provide 'Approved' or 'Rejected'.");
        //            }


        //            using (var connection = new SqlConnection(_connectionString))
        //            {
        //                connection.Open();

        //                var updateQuery = "UPDATE loanapplications SET Loanstatus = @Loanstatus WHERE ApplicationID = @ApplicationID";
        //                using (var command = new SqlCommand(updateQuery, connection))
        //                {

        //                    command.Parameters.AddWithValue("@Loanstatus", newLoanStatus);
        //                    command.Parameters.AddWithValue("@ApplicationID", id);


        //                    int rowsAffected = command.ExecuteNonQuery();


        //                    if (rowsAffected == 0)
        //                    {
        //                        return NotFound();
        //                    }
        //                }
        //            }

        //            return Ok();
        //        }

    }
    }

