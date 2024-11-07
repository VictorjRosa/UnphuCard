using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertUsuario
    {
        [Required(ErrorMessage = "El código es requerido.")]
        [Range(6, 6, ErrorMessage = "El código es de 6 Dígitos.")]
        public int UsuCodigo { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string? UsuNombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string? UsuApellido { get; set; }

        [Required(ErrorMessage = "La matricula es requerida.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "La matricula es de 7 dígitos.")]
        [RegularExpression(@"^\d{2}-\d{4}$", ErrorMessage = "La matricula debe tener el formato 12-3456.")]
        public string? UsuMatricula { get; set; }

        [Required(ErrorMessage = "El usuario es requerido.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El usuario es de 9 dígitos.")]
        [RegularExpression(@"^[a-zA-Z]{2}\d{2}-\d{4}$", ErrorMessage = "El usuario debe tener el formato ab12-3456.")]
        public string? UsuUsuario { get; set; }

        [Required(ErrorMessage = "El Correo es requerido.")]
        [StringLength(22, MinimumLength = 22, ErrorMessage = "El correo es de 22 dígitos.")]
        [RegularExpression(@"^[a-zA-Z]{2}\d{2}-\d{4}@unphu.edu.do$", ErrorMessage = "El correo debe tener el formato de ab12-3456@unphu.edu.do.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string? UsuCorreo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener 8 dígitos o más.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$", ErrorMessage = "La contraseña debe cumplir con las siguientes políticas:\n- Al menos 8 caracteres\n- Al menos una letra mayúscula\n- Al menos un número\n- Al menos un carácter especial.")]
        public string? UsuContraseña { get; set; }

        public decimal? UsuSaldo { get; set; }

        public string? UsuEstado { get; set; }

    }
}
