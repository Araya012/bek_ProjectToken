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

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([Required(ErrorMessage = "Name is required")]
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
                    return NotFound("Invalid session token");
                }

                if (session.ExpirationDate < DateTime.Now)
                {
                    return BadRequest("Session token has expired");
                }

                // Verificar si el correo electrónico ya está en uso
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
                var users = _context.bek_User.ToList();

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
