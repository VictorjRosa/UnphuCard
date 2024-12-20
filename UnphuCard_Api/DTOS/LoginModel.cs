namespace UnphuCard_Api.DTOS
{
    public class LoginModel
    {
        public string? Usuario { get; set; } 
        public string? Contraseña { get; set; } 
        public int? RolId { get; set; }
    }
}