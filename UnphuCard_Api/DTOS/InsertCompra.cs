namespace UnphuCard_Api.DTOS
{
    public class InsertCompra
    {
        public decimal? CompMonto { get; set; }
        public int? UsuCodigo { get; set; }
        public int? EstId { get; set; }
        public int? MetPagId { get; set; }
        public int? SesionId { get; set; }
    }
}