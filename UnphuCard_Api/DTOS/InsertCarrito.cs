using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertCarrito
    {
        [Required(ErrorMessage = "La sesión es requerida")]
        public int? SesionId { get; set; }
    }
}
