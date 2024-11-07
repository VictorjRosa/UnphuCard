using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class UpdateCarrito
    {
        public DateTime? CarFecha { get; set; }

        public int? CarCantidad { get; set; }

        public int? ProdId { get; set; }
    }
}
