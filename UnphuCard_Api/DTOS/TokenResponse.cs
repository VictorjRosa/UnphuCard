namespace UnphuCard_Api.DTOS
{
    public class TokenResponse
    {
        public string? Access_token { get; set; }
        public int Expires_in { get; set; }
        public int RolId { get; set; }
        public string VerificationCode { get; set; }
    }
}