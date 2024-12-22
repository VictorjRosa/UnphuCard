namespace UnphuCard_Api.DTOS
{
    public class VerificarCedulaDTO
    {
        public List<string> multiplo = new List<string>();
        public int word { get; set; } = 0;
        public string digVerificador { get; set; } = "";
        public int producto { get; set; } = 0;
        public int suma { get; set; } = 0;
        public int entero { get; set; } = 0;
        public int longitud { get; set; } = 0;
        public bool result { get; set; }
    }
}
