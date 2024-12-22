using Microsoft.AspNetCore.Mvc;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class VerificarCedula : Controller
    {
        private readonly IVerificarCedula _service;
        public VerificarCedula(IVerificarCedula service)
        {
            _service = service;
        }
    

        [HttpGet("api/VerificarCedula/{cedula}")]
        public async Task<IActionResult> VerificarCedulaAsync(string cedula)
        {
            // Llama al método VerificarCedula que tienes en tu servicio.
            bool isCedulaValid = await _service.VerificarCedula(cedula);

            if (isCedulaValid)
            {
                return Ok(isCedulaValid);  // Cédula válida
            }
            else
            {
                return Ok(isCedulaValid);
            }
        }
    }
}