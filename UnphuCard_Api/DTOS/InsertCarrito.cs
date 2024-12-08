using System.ComponentModel.DataAnnotations;

namespace UnphuCard.DTOS
{
    public class InsertCarrito
    {

        public int? CarCantidad { get; set; }

        public int? ProdId { get; set; }

        public string? SesionToken { get; set; }
    }
}
