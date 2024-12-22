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


namespace UnphuCard_Api.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioController(UnphuCardContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

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

                // Devolver el token al cliente
                return Ok(new { access_token = token });
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
