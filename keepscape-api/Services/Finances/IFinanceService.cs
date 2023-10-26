using keepscape_api.Dtos.Finances;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Finances
{
    public interface IFinanceService
    {
        Task<BalanceResponseDto?> GetBalance(Guid userId);
        Task<BalanceLogResponsePaginatedDto> GetBalanceLogs(Guid userId, PaginatorQuery paginatorQuery);
        Task<BalanceWithdrawalPaginatedResponseDto> GetBalanceWithdrawals(Guid userId, PaginatorQuery paginatorQuery);
        Task<BalanceWithdrawalResponseDto?> CreateBalanceWithdrawal(Guid userId, BalanceWithdrawalCreateDto balanceWithdrawalCreateDto);
        Task<BalanceWithdrawalPaginatedResponseDto> GetBalanceWithdrawals(BalanceWithdrawalQuery balanceWithdrawalQuery);
        Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, BalanceWithdrawalUpdateDto balanceWithdrawalUpdateDto);
    }
}
