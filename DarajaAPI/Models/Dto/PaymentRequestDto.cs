﻿using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Dto
{
    public class PaymentRequestDto
    {
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; }
    }
}