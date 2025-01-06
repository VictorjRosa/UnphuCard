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

        [HttpGet("api/MostrarUsuNombreConMatricula/{matricula}")]
        public async Task<ActionResult<Usuario>> GetUsuarioNombre(string matricula)
        {
            var usuario = await _context.Usuarios.Where(u => u.UsuMatricula == matricula).Select(u => new{u.UsuNombre, u.UsuApellido}).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            return Ok(usuario);
        }

        [HttpGet("api/MostrarUsuIdConMatricula/{matricula}")]
        public async Task<ActionResult<Usuario>> GetUsuarioId(string matricula)
        {
            var usuario = await _context.Usuarios.Where(u => u.UsuMatricula == matricula).Select(u => u.UsuId).FirstOrDefaultAsync();
            if (usuario == 0)
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
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
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
                var verificationCode = "";
                if (rolId == 1)
                {
                    var random = new Random();
                    verificationCode = random.Next(100000, 999999).ToString();

                    var subject = "Código de verificación para inicio de sesión";
                    var message = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Verificación de Inicio de Sesión UNPHU</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center' width='100%' 
           style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; margin-top: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
        
        <!-- Header with Logo -->
        <tr>
            <td style='padding: 40px 0; text-align: center; background-color: #006838; border-radius: 8px 8px 0 0;'>
                <img src='https://fotosunphucard.blob.core.windows.net/fotos/LogoUnphuBlanco.png' 
                     alt='UNPHU Logo' style='width: 200px; height: auto;'>
            </td>
        </tr>
        
        <!-- Main Content -->
        <tr>
            <td style='padding: 40px 30px;'>
                <h1 style='color: #006838; margin: 0 0 20px 0; font-size: 24px; font-weight: bold;'>
                    Hola {Usuario.UsuNombre} {Usuario.UsuApellido},
                </h1>
                
                <p style='color: #555555; font-size: 16px; line-height: 1.5; margin: 0 0 20px 0;'>
                    Se ha recibido una solicitud para iniciar sesión en tu cuenta en nuestra aplicación.
                </p>
                
                <p style='color: #555555; font-size: 16px; line-height: 1.5; margin: 0 0 30px 0;'>
                    Por favor, ingresa el siguiente código de verificación para completar el inicio de sesión:
                </p>
                
                <!-- Verification Code Box -->
                <div style='background-color: #f8f8f8; border: 2px solid #006838; border-radius: 8px; padding: 20px; text-align: center; margin: 0 0 30px 0;'>
                    <span style='font-family: ""Courier New"", monospace; font-size: 32px; font-weight: bold; color: #006838; letter-spacing: 5px;'>
                        {verificationCode}
                    </span>
                </div>
                
                <!-- Security Warning -->
                <div style='background-color: #fff8e1; border-left: 4px solid #ffa000; padding: 15px; margin: 0 0 30px 0;'>
                    <p style='color: #666666; font-size: 14px; line-height: 1.5; margin: 0;'>
                        Si no fuiste tú quien intentó iniciar sesión, por favor ignora este mensaje. 
                        En caso de que no hayas realizado esta solicitud, te recomendamos cambiar tu contraseña 
                        para asegurar la seguridad de tu cuenta.
                    </p>
                </div>
                
                <p style='color: #555555; font-size: 16px; line-height: 1.5; margin: 0;'>
                    Si tienes alguna duda o inquietud, no dudes en contactarnos.
                </p>
            </td>
        </tr>
        
        <!-- Footer -->
        <tr>
            <td style='padding: 30px; background-color: #00A650; border-radius: 0 0 8px 8px; text-align: center;'>
                <p style='color: #ffffff; font-size: 14px; margin: 0;'>
                    © {fechaEnRD} Universidad Nacional Pedro Henríquez Ureña
                </p>
                <p style='color: #ffffff; font-size: 12px; margin: 10px 0 0 0;'>
                    Ave. John F. Kennedy Km. 7 1/2, Santo Domingo, República Dominicana
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";

                    try
                    {
                        await _emailService.SendEmailAsync(Usuario.UsuCorreo ?? "", subject, message);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = "Error al enviar el correo electrónico de verificación.";
                        return StatusCode(500, new { Error = errorMessage });
                    }
                }

                // Devolver el token al cliente
                return Ok(new { access_token = token, rolId = login.RolId, VerificationCode = verificationCode });
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
