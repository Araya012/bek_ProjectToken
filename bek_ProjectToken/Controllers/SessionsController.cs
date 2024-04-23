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
        //The _context field is set to access the database context.
        private readonly YourDbContext _context;

        //A constant SessionExpirationMinutes is also defined that determines the duration of the session in minutes.
        private const int SessionExpirationMinutes = 1;

        //Finally, a constructor is defined that accepts a parameter of type YourDbContext and assigns it to the _context field.
        public SessionsController(YourDbContext context)
        {
            _context = context;
        }

        //HTTP GET Request
        /*This GetSession method is invoked when a GET request is received on the path "/api/sessions/{sessionId}". 
         * Searches the database for the session corresponding to the given ID. If the session is not found, it returns a 404 (Not Found) status code with a descriptive message.
         * If found, it returns a 200 (OK) status code along with the session data.*/
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
        /*Takes a clientKey parameter, which is expected to be provided in the body of the POST request. 
        * This parameter represents the client key that will be used to identify the client in the system.*/
        [HttpPost]
        public IActionResult CreateClientSession(string clientKey)
        {
            try
            {
                //In this block of code, the database is searched for a customer that matches the key provided
                var client = _context.bek_Client.FirstOrDefault(c => c.ClientKey == clientKey);
                
                //If client is not found, it returns error 404
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                //In this block of code, the database is searched for an existing session associated with the previously found client.
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
