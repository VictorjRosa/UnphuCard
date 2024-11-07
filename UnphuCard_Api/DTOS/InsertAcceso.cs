using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertAcceso
    {
        [Required(ErrorMessage = "La fecha del acceso es requerida")]
        public DateTime? AccesFecha { get; set; }

        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int? UsuId { get; set; }

        [Required(ErrorMessage = "El ID del aula es requerido")]
        public int? AulaId { get; set; }

        [Required(ErrorMessage = "El ID del estado es requerido")]
        public int? StatusId { get; set; }
    }
}
