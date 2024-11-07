using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertSesion
    {
        [Required(ErrorMessage = "El Tóken de la sesión es requerido")]
        public string? SesionToken { get; set; }

        [Required(ErrorMessage = "La fecha de la sesión es requerida")]
        public DateTime? SesionFecha { get; set; }

        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int? UsuId { get; set; }
    }
}
