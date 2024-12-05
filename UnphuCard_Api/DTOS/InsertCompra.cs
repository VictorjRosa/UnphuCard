using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertCompra
    {
        [Required(ErrorMessage = "El monto de la compra es requerido")]
        public decimal? CompMonto { get; set; }

        [Required(ErrorMessage = "El código de usuario es requerido")]
        public int? UsuCodigo { get; set; }

        [Required(ErrorMessage = "El ID del establecimiento es requerido")]
        public int? EstId { get; set; }

        [Required(ErrorMessage = "El ID del método de pago es requerido")]
        public int? MetPagId { get; set; }

        [Required(ErrorMessage = "El ID de la sesión es requerido")]
        public int? SesionId { get; set; }
    }
}
