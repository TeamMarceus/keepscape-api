﻿using keepscape_api.Enums;

namespace keepscape_api.Dtos.Finances
{
    public record BalanceWithdrawalResponseDto
    {
        public Guid Id { get; init; }
        public Guid BalanceId { get; set; }
        public Guid SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = Enums.PaymentMethod.BankTransfer.ToString();
        public string PaymentDetails { get; set; } = string.Empty;
        public string PaymentProfileImageUrl { get; set; } = string.Empty;
        public string? PaymentProofImageUrl { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string Status { get; set; } = PaymentStatus.Pending.ToString();
    }

    public record BalanceWithdrawalPaginatedResponseDto
    {
        public IEnumerable<BalanceWithdrawalResponseDto> BalanceWithdrawals { get; init; } = new List<BalanceWithdrawalResponseDto>();
        public int PageCount { get; init; } = 0;
    }

    public record BalanceLogResponseDto
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public string Remarks { get; init; } = string.Empty;
    }
    public record BalanceResponseDto
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public IEnumerable<BalanceLogResponseDto> Histories { get; init; } = new List<BalanceLogResponseDto>();
        public IEnumerable<BalanceWithdrawalResponseDto> Withdrawals { get; init; } = new List<BalanceWithdrawalResponseDto>();
    }
}