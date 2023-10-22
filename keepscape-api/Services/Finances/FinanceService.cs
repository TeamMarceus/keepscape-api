using AutoMapper;
using keepscape_api.Dtos.Finances;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;

namespace keepscape_api.Services.Finances
{
    public class FinanceService : IFinanceService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBalanceWithdrawalRepository _balanceWithdrawalRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public FinanceService(
            IUserRepository userRepository,
            IBalanceRepository balanceRepository,
            IBalanceWithdrawalRepository balanceWithdrawalRepository,
            IImageService imageService,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _balanceRepository = balanceRepository;
            _balanceWithdrawalRepository = balanceWithdrawalRepository;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<BalanceWithdrawalResponseDto?> CreateBalanceWithdrawal(Guid userId, BalanceWithdrawalCreateDto balanceWithdrawalCreateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.Balance == null)
            {
                return null;
            }

            var balance = await _balanceRepository.GetByIdAsync(user.Balance.Id);

            if (balance == null)
            {
                return null;
            }

            if (balance.Amount < balanceWithdrawalCreateDto.Amount)
            {
                return null;
            }

            balance.Amount -= balanceWithdrawalCreateDto.Amount;
            balance.Histories.Add(new BalanceLog
            {
                Amount = -balanceWithdrawalCreateDto.Amount,
                Remarks = "Withdrawal Requested",
            });

            var balanceWithdrawal = new BalanceWithdrawal
            {
                Amount = balanceWithdrawalCreateDto.Amount,
                PaymentMethod = Enum.Parse<PaymentMethod>(balanceWithdrawalCreateDto.PaymentMethod),
                PaymentDetails = balanceWithdrawalCreateDto.PaymentDetails,
                PaymentProfileImageUrl = await _imageService.Upload("payment-profile", balanceWithdrawalCreateDto.PaymentProfileImage) ?? "",
                Remarks = balanceWithdrawalCreateDto.Remarks,
                Status = PaymentStatus.Pending,
            };

            balance.Withdrawals.Add(balanceWithdrawal);
   
            return await _balanceRepository.UpdateAsync(balance) ? _mapper.Map<BalanceWithdrawalResponseDto>(balanceWithdrawal) : null;
        }

        public async Task<BalanceResponseDto?> GetBalance(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.Balance == null)
            {
                return null;
            }

            var balance = await _balanceRepository.GetByIdAsync(user.Balance.Id);

            return _mapper.Map<BalanceResponseDto>(balance);
        }

        public async Task<IEnumerable<BalanceWithdrawalResponseDto>> GetBalanceWithdrawals()
        {
            var balanceWithdrawals = await _balanceWithdrawalRepository.GetAllAsync();

            return balanceWithdrawals.Select(balanceWithdrawal => _mapper.Map<BalanceWithdrawalResponseDto>(balanceWithdrawal));
        }

        public async Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, BalanceWithdrawalUpdateDto balanceWithdrawalUpdateDto)
        {
            var enumParse = Enum.TryParse(balanceWithdrawalUpdateDto.Status, out PaymentStatus status);

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
                balance.Histories!.Add(new BalanceLog
                {
                    Amount = 0,
                    Remarks = "Withdrawal Approved",
                });
            } 
            else if (status == PaymentStatus.Rejected)
            {
                balance.Amount += balanceWithdrawal.Amount;
                balance.Histories!.Add(new BalanceLog
                {
                    Amount = +balanceWithdrawal.Amount,
                    Remarks = "Withdrawal Rejected",
                });
            }
            balanceWithdrawal.Status = status;
            balanceWithdrawal.PaymentProofImageUrl = balanceWithdrawalUpdateDto.PaymentProofImage != null ? 
                await _imageService.Upload("payment-proof", balanceWithdrawalUpdateDto.PaymentProofImage) : null;

            return await _balanceWithdrawalRepository.UpdateAsync(balanceWithdrawal) && await _balanceRepository.UpdateAsync(balance);
        }
    }
}
