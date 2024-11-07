﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class EstablecimientoController : Controller
    {
        private readonly UnphuCardContext _context;
        public EstablecimientoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarEstablecimientos")]
        public async Task<ActionResult<IEnumerable<Establecimiento>>> GetEstablecimientos()
        {
            return await _context.Establecimientos.ToListAsync();
        }

        [HttpGet("api/MostrarEstablecimiento/{id}")]
        public async Task<ActionResult<Establecimiento>> GetEstablecimiento(int id)
        {
            var establecimiento = await _context.Establecimientos.FirstOrDefaultAsync(e => e.EstId == id);
            if (establecimiento == null)
            {
                return BadRequest("Establecimiento no encontrado");
            }
            return establecimiento;
        }
    }
}
