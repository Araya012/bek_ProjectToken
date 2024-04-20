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
                // Busca la sesión en la base de datos
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionId == sessionId);

                if (session == null)
                {
                    return NotFound("Session not found");
                }

                // Retorna la sesión encontrada
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
                // Validar la autenticidad de la llave del cliente contra la tabla de clientes
                var client = _context.bek_Client.FirstOrDefault(c => c.ClientKey == clientKey);
                if (client == null)
                {
                    return NotFound("Invalid client key");
                }

                // Generar una sesión usando un algoritmo personalizado para encriptar y desencriptar un texto
                string sessionToken = GenerateSessionToken();

                // Almacenar la sesión en la tabla de sesiones
                var session = new Session
                {
                    ClientId = client.ClientId,
                    SessionToken = sessionToken,
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(1) // Ejemplo: sesión válida por 1 día
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
            // Aquí implementarías tu algoritmo personalizado para generar el token de sesión
            // Por ejemplo, puedes usar clases como Random o algún algoritmo de cifrado
            // En este ejemplo, simplemente genero un GUID como token de sesión
            return Guid.NewGuid().ToString();
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(string name, string email, string sessionToken)
        {
            try
            {
                // Validar la autenticidad de la sesión contra la tabla de sesiones
                var session = _context.bek_Session.FirstOrDefault(s => s.SessionToken == sessionToken);
                if (session == null || session.ExpirationDate < DateTime.Now)
                {
                    return NotFound("Invalid session token or session expired");
                }

                // Crear el usuario
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
                // Recupera todos los usuarios de la tabla bek_User
                var users = _context.bek_User.ToList();

                if (users == null || users.Count == 0)
                {
                    // No se encontraron usuarios en la base de datos
                    return NotFound("No users found");
                }

                // Retorna la lista de usuarios
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Ocurrió un error al intentar recuperar los usuarios
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
