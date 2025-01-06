namespace UnphuCard_Api.DTOS
{
    public class InsertRecarga
    {
        public int Rec_ID { get; set; }
        public decimal Rec_Monto { get; set; }
        public DateTime Rec_Fecha { get; set; }
        public int Usu_ID { get; set; }
        public int MetPag_ID { get; set; }
    }
}
