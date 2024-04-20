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
        private const int SessionExpirationMinutes = 1;

        public SessionsController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet("{sessionId}")]
        public IActionResult GetSession(int sessionId)
        {
            try
            {
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionId == sessionId);
                if (session == null)
                {
                    return NotFound("Session not found");
                }
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
                var client = _context.bek_Client.FirstOrDefault(c => c.ClientKey == clientKey);
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                Session session = _context.bek_Session.FirstOrDefault(s => s.ClientId == client.ClientId);
                if (session != null)
                {
                    UpdateSession(session);
                    return Ok(new { SessionToken = session.SessionToken });
                }

                string sessionToken = GenerateSessionToken();
                session = new Session
                {
                    ClientId = client.ClientId,
                    SessionToken = sessionToken,
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMinutes(SessionExpirationMinutes)
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

        private void UpdateSession(Session session)
        {
            session.SessionToken = GenerateSessionToken();
            session.CreationDate = DateTime.Now;
            session.ExpirationDate = DateTime.Now.AddMinutes(SessionExpirationMinutes);
            _context.SaveChanges();
        }

        private string GenerateSessionToken()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
