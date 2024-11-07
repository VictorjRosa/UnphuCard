using Microsoft.AspNetCore.Mvc;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class RecargasController : Controller
    {
        private readonly UnphuCardContext _context;
        public RecargasController(UnphuCardContext context)
        {
            _context = context;
        }

        //[HttpPost("api/RegistrarRecargas")]
        //public async Task<ActionResult> PostRecargas([FromBody] Recarga recarga)
        //{

        //}
    }
}
