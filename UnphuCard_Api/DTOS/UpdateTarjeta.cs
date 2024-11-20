using System.ComponentModel.DataAnnotations;

namespace UnphuCard_Api.DTOS
{
    public class UpdateTarjeta
    {
        [Required(ErrorMessage = "El estado de la tarjeta es requerido")]
        public int? StatusId { get; set; }
    }
}