﻿namespace UnphuCard_Api.DTOS
{
    public class PagoRequest
    {
        public decimal Amount { get; set; }
        public string? OrderNumber { get; set; }
        public string? CardNumber { get; set; }
        public string? CardExpirationDate { get; set; }
        public string? CardCvv { get; set; }
        public string? Description { get; set; }
        public string? Customer { get; set; }
        public int UsuarioId { get; set; }
        public int MetodoPago { get; set; }
        public string? Token { get; set; }
    }
}