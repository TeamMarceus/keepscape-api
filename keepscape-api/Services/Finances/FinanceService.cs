using AutoMapper;
using keepscape_api.Dtos.Finances;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Services.Finances
{
    public class FinanceService : IFinanceService
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBalanceWithdrawalRepository _balanceWithdrawalRepository;
        private readonly IMapper _mapper;
        public FinanceService(
                IBalanceRepository balanceRepository,
                IBalanceWithdrawalRepository balanceWithdrawalRepository,
                IMapper mapper
                )
        {
            _balanceRepository = balanceRepository;
            _balanceWithdrawalRepository = balanceWithdrawalRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BalanceWithdrawalResponseDto>> GetBalanceWithdrawals()
        {
            var balanceWithdrawals = await _balanceWithdrawalRepository.GetAllAsync();

            return balanceWithdrawals.Select(balanceWithdrawal => _mapper.Map<BalanceWithdrawalResponseDto>(balanceWithdrawal));
        }

        public async Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, string paymentStatus)
        {
            var enumParse = Enum.TryParse(paymentStatus, out PaymentStatus status);

            if (!enumParse)
            {
                return false;
            }

            var balanceWithdrawal = await _balanceWithdrawalRepository.GetByIdAsync(balanceWithdrawalId);

            if (balanceWithdrawal == null)
            {
                return false;
            }
            var balance = await _balanceRepository.GetByIdAsync(balanceWithdrawal.BalanceId);

            if (balance == null)
            {
                return false;
            }

            if (status == PaymentStatus.Paid)
            {
                balance.BalanceHistories!.Add(new BalanceLog
                {
                    Amount = -balanceWithdrawal.Amount,
                    Remarks = "Withdrawal Approved",
                });
            } 
            else if (status == PaymentStatus.Rejected)
            {
                balance.Amount = balance.Amount + balanceWithdrawal.Amount;
                balance.BalanceHistories!.Add(new BalanceLog
                {
                    Amount = +balanceWithdrawal.Amount,
                    Remarks = "Withdrawal Rejected",
                });
            }
            balanceWithdrawal.Status = status;

            return await _balanceWithdrawalRepository.UpdateAsync(balanceWithdrawal) && await _balanceRepository.UpdateAsync(balance);
        }
    }
}
