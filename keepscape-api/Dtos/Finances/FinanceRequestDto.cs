﻿using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Finances
{
    public record BalanceWithdrawalUpdateDto
    {
        [Required]
        public string Status { get; init; } = string.Empty;
        public string? Reason { get; init; }
        public IFormFile? PaymentProofImage { get; init; }
    }

    public record BalanceWithdrawalCreateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public decimal Amount { get; init; }
        [Required]
        public string PaymentMethod { get; init; } = string.Empty;
        [Required]
        public string PaymentDetails { get; init; } = string.Empty;
        public IFormFile? PaymentProfileImage { get; init; }
        public string? Remarks { get; init; }
    }   
}
    