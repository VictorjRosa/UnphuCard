using System.ComponentModel.DataAnnotations;

namespace UnphuCard_Api.DTOS
{
    public class UpdateInscripcion
    {
        [Required(ErrorMessage = "El estado de la inscripción es requerido")]
        public int? StatusId { get; set; }
    }
}
