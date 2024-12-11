namespace UnphuCard_Api.DTOS
{
    public class UpdateProducto
    {
        public string? ProdDescripcion { get; set; }
        public decimal? ProdPrecio { get; set; }
        public string? ProdImagenes { get; set; }
        public int? StatusId { get; set; }
    }
}