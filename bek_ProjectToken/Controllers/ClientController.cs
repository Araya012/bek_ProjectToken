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
        private readonly YourDbContext _context;

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
                                          string sessionToken)
        {
            try
            {
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionToken == sessionToken);
                if (session == null)
                {
                 // Verify token session 
                    return NotFound("Invalid session token");
                }

                // Verify token expiration
                if (session.ExpirationDate < DateTime.Now)
                {
                    return BadRequest("Session token has expired");
                }

                // Check if the email is in use
                var existingUser = _context.bek_User.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    return Conflict("Email already in use");
                }

                var user = new User
                {
                    Name = name,
                    Email = email
                };

                //Save user
                _context.bek_User.Add(user);
                _context.SaveChanges();
                
                // Return Code 200
                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                //HTTP Response
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
