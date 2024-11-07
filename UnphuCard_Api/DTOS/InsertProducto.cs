using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertProducto
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        public string? ProdDescripcion { get; set; }

        [Required(ErrorMessage = "El precio del producto es requerido")]
        public decimal? ProdPrecio { get; set; }
        
        public string? ProdImagenes { get; set; }

        [Required(ErrorMessage = "El estado del producto es requerido")]
        public int? StatusId { get; set; }

        [Required(ErrorMessage = "La categoria del producto es requerida")]
        public int? CatProdId { get; set; }
    }
}
