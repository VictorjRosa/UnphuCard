using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using UnphuCard_Api.Models;
using UnphuCard_Api.DTOS;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using UnphuCard_Api.Service;


namespace UnphuCard_Api.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IConfiguration _configuration;
        private readonly IServicioEmail _emailService;

        public UsuarioController(UnphuCardContext context, IConfiguration configuration, IServicioEmail emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet("api/MostrarUsuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("api/MostrarUsuario/{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuId == id);
            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            return usuario;
        }

        [HttpGet("api/MostrarUsuarioNombre/{id}")]
        public async Task<ActionResult<Usuario>> GetUsuarioNombre(int id)
        {
            var usuario = await _context.Usuarios.Where(u => u.UsuId == id).Select(u => u.UsuNombre + " " + u.UsuApellido).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            return Ok(usuario);
        }

        [HttpGet("api/ObtenerEstadoId/{cedula}")]
        public async Task<ActionResult<Usuario>> GetEstadoId(string cedula)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuDocIdentidad == cedula);
            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            return Ok(usuario.StatusId);
        }

        [HttpPost("api/Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                string errorMessage = "";
                var rolId = await _context.Usuarios.Where(u => u.UsuUsuario == login.Usuario).Select(u => u.RolId).FirstOrDefaultAsync();
                if (login.RolId is null)
                {
                    if (rolId == 1)
                    {
                        login.RolId = rolId;
                    }
                    else if (rolId == 3)
                    {
                        login.RolId = rolId;
                    }
                }
                // Verifica si el usuario existe en la base de datos
                var Usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuUsuario == login.Usuario && u.RolId == login.RolId);
                if (Usuario == null || Usuario.RolId != login.RolId)
                {
                    return Unauthorized("Credenciales inválidas.");
                }
                // Generar el token JWT
                var token = GenerateJwtToken(Usuario);

                var random = new Random();
                var verificationCode = random.Next(100000, 999999).ToString();

                var subject = "Código de verificación para inicio de sesión";
                var message = $"<h1>Hola {Usuario.UsuNombre} {Usuario.UsuApellido}, </h1>" +
                              $"<p>Se ha recibido una solicitud para iniciar sesión en tu cuenta en nuestra aplicación.</p>" +
                              $"<p>Por favor, ingresa el siguiente código de verificación para completar el inicio de sesión:</p>" +
                              $"<h2><strong>{verificationCode}</strong></h2>" +
                              $"<p>Si no fuiste tú quien intentó iniciar sesión, por favor ignora este mensaje. En caso de que no hayas realizado esta solicitud, te recomendamos cambiar tu contraseña para asegurar la seguridad de tu cuenta.</p>" +
                              $"<p>Si tienes alguna duda o inquietud, no dudes en contactarnos.</p>";

                try
                {
                    await _emailService.SendEmailAsync(Usuario.UsuCorreo ?? "", subject, message);
                }
                catch (Exception ex)
                {
                    errorMessage = "Error al enviar el correo electrónico de verificación.";
                    return StatusCode(500, new { Error = errorMessage });
                }

                // Devolver el token al cliente
                return Ok(new { access_token = token, rolId = login.RolId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuUsuario),
            new Claim("UserId", usuario.UsuId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["TokenExpiryInMinutes"])),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
