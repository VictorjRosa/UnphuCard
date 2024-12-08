using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertInventario
    {
        [Required(ErrorMessage = "La cantidad del inventario es requerida")]
        public int? InvCantidad { get; set; }

        [Required(ErrorMessage = "El ID del establecimiento es requerido")]
        public int? EstId { get; set; }

        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int? ProdId { get; set; }
    }
}
