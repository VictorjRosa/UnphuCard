namespace UnphuCard_Api.DTOS
{
    public class InsertUsuario
    {
        public int UsuCodigo { get; set; }
        public string? UsuNombre { get; set; }
        public string? UsuApellido { get; set; }
        public string? UsuMatricula { get; set; }
        public string? UsuUsuario { get; set; }
        public string? UsuCorreo { get; set; }
        public string? UsuContraseña { get; set; }
        public decimal? UsuSaldo { get; set; }
        public string? UsuEstado { get; set; }
    }
}