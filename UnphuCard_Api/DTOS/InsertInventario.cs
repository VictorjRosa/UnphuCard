namespace UnphuCard_Api.DTOS
{
    public class InsertInventario
    {
        public int? InvCantidad { get; set; }
        public int? EstId { get; set; }
        public string? ProdDescripcion { get; set; }
        public int? ProdPrecio { get; set; }
        public int? StatusId { get; set; }
        public int? CatProdId { get; set; }
    }
}