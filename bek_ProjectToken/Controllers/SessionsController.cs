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
        //Active session time
        private const int SessionExpirationMinutes = 1;

        public SessionsController(YourDbContext context)
        {
            _context = context;
        }

        //HTTP GET Request
        [HttpGet("{sessionId}")]
        public IActionResult GetSession(int sessionId)
        {
            try
            {
                //Session Validation
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionId == sessionId);
                if (session == null)
                {
                    //If session not found, it returns error 404
                    return NotFound("Session not found");
                }
                //If session found, it return code 200
                return Ok(session);
            }
            catch (Exception ex)
            {
                //HTTP Response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Request to create a new HTTP POST client session
        [HttpPost]
        public IActionResult CreateClientSession(string clientKey)
        {
            try
            {
                var client = _context.bek_Client.FirstOrDefault(c => c.ClientKey == clientKey);
                
                //If client is not found, it returns error 404
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                Session session = _context.bek_Session.FirstOrDefault(s => s.ClientId == client.ClientId);
                if (session != null)
                {
                    //If client session is found, update currently session
                    UpdateSession(session);
                    return Ok(new { SessionToken = session.SessionToken });
                }
                // If session client is not found, generate new session token
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

                //Return Code 200
                return Ok(new { SessionToken = sessionToken });
            }
            catch (Exception ex)
            {
                //HTTP Response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Update Currently session
        private void UpdateSession(Session session)
        {
            session.SessionToken = GenerateSessionToken();
            session.CreationDate = DateTime.Now;
            session.ExpirationDate = DateTime.Now.AddMinutes(SessionExpirationMinutes);
            _context.SaveChanges();
        }

        //Create new session token
        private string GenerateSessionToken()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
