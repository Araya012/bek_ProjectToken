using apiv2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace apiv2.Controllers
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

        [HttpPost]
        public IActionResult CreateClientSession(string clientKey)
        {
            try
            {
                // Validate the authenticity of the client key against the clients table
                var client = _context.Clients.FirstOrDefault(c => c.ClientKey == clientKey);
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                // Generate a session using a custom algorithm to encrypt and decrypt a text
                string sessionToken = GenerateSessionToken();

                // Store the session in the sessions table
                var session = new Session
                {
                    ClientId = client.ClientId,
                    SessionToken = sessionToken,
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(1) // Example: session valid for 1 day
                };
                _context.Sessions.Add(session);
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
            // In this example, simply generate a GUID as the session token
            return Guid.NewGuid().ToString();
        }
    }
}
