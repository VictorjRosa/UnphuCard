using System.ComponentModel.DataAnnotations;

namespace UnphuCard_Api.DTOS
{
    public class UpdateTarjetaProv
    {
        [Required(ErrorMessage = "El estado de la tarjeta provisional es requerido")]
        public int? StatusId { get; set; }

        [Required(ErrorMessage = "El documento de identidad para la tarjeta provisional es requerido")]
        public string? UsuDocIdentidad { get; set; }
    }
}