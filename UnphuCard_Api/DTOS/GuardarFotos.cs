namespace UnphuCard_Api.DTOS
{
    public class GuardarFotos
    {
        public IFormFile? Archivo { get; set; }
        public string? Ruta { get; set; }
    }
}