using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertSesion
    {
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int? UsuId { get; set; }
    }
}
