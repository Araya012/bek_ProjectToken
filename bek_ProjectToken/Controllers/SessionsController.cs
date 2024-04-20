using bek_ProjectToken.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace bek_ProjectToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly YourDbContext _context;

        public SessionsController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet("{sessionId}")]
        public IActionResult GetSession(int sessionId)
        {
            try
            {
                // Find the session in the database
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionId == sessionId);

                if (session == null)
                {
                    return NotFound("Session not found");
                }

                // Return the found session
                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateClientSession(string clientKey)
        {
            try
            {
                // Validate the authenticity of the client key against the Clients table
                var client = _context.bek_Client.FirstOrDefault(c => c.ClientKey == clientKey);
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                // Generate a session using a custom algorithm to encrypt and decrypt text
                string sessionToken = GenerateSessionToken();

                // Store the session in the Sessions table
                var session = new Session
                {
                    ClientId = client.ClientId,
                    SessionToken = sessionToken,
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(1) // Example: session valid for 1 day
                };
                _context.bek_Session.Add(session);
                _context.SaveChanges();

                return Ok(new { SessionToken = sessionToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateSessionToken()
        {
            // Here you would implement your custom algorithm to generate the session token
            // For example, you can use classes like Random or some encryption algorithm
            // In this example, I'm simply generating a GUID as the session token
            return Guid.NewGuid().ToString();
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(string name, string email, string sessionToken)
        {
            try
            {
                // Validate the authenticity of the session against the Sessions table
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionToken == sessionToken);
                if (session == null || session.ExpirationDate < DateTime.Now)
                {
                    return NotFound("Invalid session token or session expired");
                }

                // Create the user
                var user = new User
                {
                    Name = name,
                    Email = email
                };
                _context.bek_User.Add(user);
                _context.SaveChanges();

                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                // Retrieve all users from the bek_User table
                var users = _context.bek_User.ToList();

                if (users == null || users.Count == 0)
                {
                    // No users found in the database
                    return NotFound("No users found");
                }

                // Return the list of users
                return Ok(users);
            }
            catch (Exception ex)
            {
                // An error occurred while trying to retrieve the users
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
