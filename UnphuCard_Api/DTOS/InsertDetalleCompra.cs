using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertDetalleCompra
    {
        [Required(ErrorMessage = "La cantidad de la compra es requerida")]
        public int? DetCompCantidad { get; set; }

        [Required(ErrorMessage = "El precio de la compra es requerido")]
        public decimal? DetCompPrecio { get; set; }

        [Required(ErrorMessage = "El ID de la compra es requerido")]
        public int? CompId { get; set; }

        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int? ProdId { get; set; }

        [Required(ErrorMessage = "El ID de la sesión es requerido")]
        public int? SesionId { get; set; }
    }
}
