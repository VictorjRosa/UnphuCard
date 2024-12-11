namespace UnphuCard_Api.DTOS
{
    public class InsertProducto
    {
        public string? ProdDescripcion { get; set; }
        public decimal? ProdPrecio { get; set; }
        public string? ProdImagenes { get; set; }
        public int? StatusId { get; set; }
        public int? CatProdId { get; set; }
    }
}