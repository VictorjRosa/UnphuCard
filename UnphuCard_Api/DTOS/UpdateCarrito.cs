using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class UpdateCarrito
    {

        public int? CarCantidad { get; set; }

        public int? ProdId { get; set; }
    }
}
