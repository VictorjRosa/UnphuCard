namespace UnphuCard_Api.DTOS
{
    public class InsertDetalleCompra
    {
        public int? DetCompCantidad { get; set; }
        public decimal? DetCompPrecio { get; set; }
        public int? CompId { get; set; }
        public int? ProdId { get; set; }
        public int? SesionId { get; set; }
        public int? EstId { get; set; }
    }
}