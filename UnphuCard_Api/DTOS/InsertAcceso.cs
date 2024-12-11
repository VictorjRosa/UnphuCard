namespace UnphuCard_Api.DTOS
{
    public class InsertAcceso
    {
        public DateTime? AccesFecha { get; set; }
        public int? UsuId { get; set; }
        public int? AulaId { get; set; }
        public int? StatusId { get; set; }
    }
}