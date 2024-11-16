namespace UnphuCard_Api.DTOS
{
    public class UpdateTarjetaProv
    {
        public DateTime? TarjProvFecha { get; set; }

        public DateTime? TarjProvFechaExpiracion { get; set; }

        public int? StatusId { get; set; }

        public int? UsuId { get; set; }
    }
}
