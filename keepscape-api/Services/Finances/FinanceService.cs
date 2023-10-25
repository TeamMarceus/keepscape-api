using AutoMapper;
using keepscape_api.Dtos.Finances;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.Emails;

namespace keepscape_api.Services.Finances
{
    public class FinanceService : IFinanceService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBalanceWithdrawalRepository _balanceWithdrawalRepository;
        private readonly IImageService _imageService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public FinanceService(
            IUserRepository userRepository,
            IBalanceRepository balanceRepository,
            IBalanceWithdrawalRepository balanceWithdrawalRepository,
            IImageService imageService,
            IEmailService emailService,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _balanceRepository = balanceRepository;
            _balanceWithdrawalRepository = balanceWithdrawalRepository;
            _imageService = imageService;
            _emailService = emailService;
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

        public async Task<BalanceWithdrawalPaginatedResponseDto> GetBalanceWithdrawals(BalanceWithdrawalQuery balanceWithdrawalQuery)
        {
            var balanceWithdrawalsQuery = await _balanceWithdrawalRepository.Get(balanceWithdrawalQuery);

            return new BalanceWithdrawalPaginatedResponseDto
            {
                BalanceWithdrawals = balanceWithdrawalsQuery.BalanceWithdrawals.Select(balanceWithdrawal => _mapper.Map<BalanceWithdrawalResponseDto>(balanceWithdrawal)),
                PageCount = balanceWithdrawalsQuery.PageCount
            };
        }

        public async Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, BalanceWithdrawalUpdateDto balanceWithdrawalUpdateDto)
        {
            var enumParse = Enum.TryParse(balanceWithdrawalUpdateDto.Status, out PaymentStatus status);

            if (!enumParse)
            {
                return false;
            }

            var balanceWithdrawal = await _balanceWithdrawalRepository.GetByIdAsync(balanceWithdrawalId);

            if (balanceWithdrawal == null || balanceWithdrawal.Status != PaymentStatus.Pending)
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

                var emailString = $"<p>Your withdrawal request has been approved.</p>" +
                    $"<p>Please wait for 3-5 business days for the money to be transferred to your account.</p>" +
                    $"<p>Thank you for using Keepscape.</p>";
                await _emailService.SendEmailAsync(balanceWithdrawal.Balance!.User!.Email, "Withdrawal Approved", emailString);

            } 
            else if (status == PaymentStatus.Rejected)
            {
                balance.Amount += balanceWithdrawal.Amount;
                balance.Histories!.Add(new BalanceLog
                {
                    Amount = +balanceWithdrawal.Amount,
                    Remarks = $"Withdrawal Rejected, Reason: {balanceWithdrawalUpdateDto.Reason}",
                });

                var emailString = $"<p>Your withdrawal request has been rejected.</p>" +
                    $"<p>Reason: {balanceWithdrawalUpdateDto.Reason}.</p>" +
                    $"Please contact us for more information. Thank you.</p>";
                await _emailService.SendEmailAsync(balanceWithdrawal.Balance!.User!.Email, "Withdrawal Rejected", emailString);  
            }
            balanceWithdrawal.Status = status;
            balanceWithdrawal.PaymentProofImageUrl = balanceWithdrawalUpdateDto.PaymentProofImage != null ? 
                await _imageService.Upload("payment-proof", balanceWithdrawalUpdateDto.PaymentProofImage) : null;

            return await _balanceWithdrawalRepository.UpdateAsync(balanceWithdrawal) && await _balanceRepository.UpdateAsync(balance);
        }
    }
}
