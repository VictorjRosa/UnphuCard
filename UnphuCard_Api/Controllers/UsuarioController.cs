using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UnphuCardContext _context;
        public UsuarioController(UnphuCardContext context)
        {
            _context = context;
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
    }
}
