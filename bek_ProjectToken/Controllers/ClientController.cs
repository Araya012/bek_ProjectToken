using bek_ProjectToken.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace bek_ProjectToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        /*A private field called _context of type YourDbContext is defined, which represents the database context used to interact with the database. 
         * This context is passed to the controller's constructor to allow dependency injection.*/
        private readonly YourDbContext _context;

        /*A constructor is defined that accepts a parameter of type YourDbContext and assigns it to the _context field.
         * This makes it easier to inject database context dependencies into the controller.*/
        public ClientController(YourDbContext context)
        {
            _context = context;
        }
        // Create user by HTTPPOST request
        [HttpPost("CreateUser")]
        public IActionResult CreateUser([Required(ErrorMessage = "Name is required")]
                                          //Condicionals for user creation
                                          [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Name can only contain letters")]
                                          string name,
                                          [Required(ErrorMessage = "Email is required")]
                                          [EmailAddress(ErrorMessage = "Invalid email format")]
                                          string email,
                                          string sessionToken)// Session token for authentication
        {
            try
            {
                // Check the existence and validity of the session token in the database
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionToken == sessionToken);
                if (session == null)
                {
                    // If the session token is invalid, returns a 404 (Not Found) status code
                    return NotFound("Invalid session token");
                }

                // Check if the session token has expired
                if (session.ExpirationDate < DateTime.Now)
                {
                    // If the session token has expired, returns a 400 (Bad Request) status code
                    return BadRequest("Session token has expired");
                }

                // Check if the provided email is already in use by another user
                var existingUser = _context.bek_User.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    // If the email is already in use, returns a 409 (Conflict) status code
                    return Conflict("Email already in use");
                }

                // Create a new user object with the given name and email
                var user = new User
                {
                    Name = name,
                    Email = email
                };

                // Save the new user to the database
                _context.bek_User.Add(user);
                _context.SaveChanges();

                // Return a 200 (OK) status code along with a success message
                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                // If an error occurs during the process, returns a status code 500 (Internal Server Error)
                // along with a message indicating the specific error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //HTTP Get request to fetch store users
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.bek_User.ToList();

                //If no users are found, it returns error 404
                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found");
                }
                //If user are found, it returns code 200 and list of users 
                return Ok(users);
            }
            catch (Exception ex)
            {
                //HTTP Response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
