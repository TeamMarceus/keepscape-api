using AutoMapper;
using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.ConfirmationCodes;
using keepscape_api.Services.Emails;
using keepscape_api.Services.Tokens;
using Microsoft.AspNetCore.Identity;

namespace keepscape_api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository<BuyerProfile> _buyerProfileRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ISellerApplicationRepository _sellerApplicationRepository;
        private readonly IImageService _baseImageService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository,          
            IProfileRepository<BuyerProfile> buyerProfileRepository, 
            ISellerProfileRepository sellerProfileRepository, 
            ISellerApplicationRepository sellerApplicationRepository,
            IImageService baseImageService,
            ITokenService tokenService,
            IEmailService emailService,
            IConfirmationCodeService confirmationCodeService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _sellerApplicationRepository = sellerApplicationRepository;
            _baseImageService = baseImageService;
            _tokenService = tokenService;
            _emailService = emailService;
            _confirmationCodeService = confirmationCodeService;
            _mapper = mapper;
        }

        public async Task<UserSellerApplicationDto?> GetApplication(Guid userId)
        {
            var sellerProfile = await _sellerProfileRepository.GetProfileByUserGuid(userId);

            if (sellerProfile == null)
            {
                return null;
            }

            if (sellerProfile.SellerApplication == null)
            {
                return null;
            }

            return _mapper.Map<UserSellerApplicationDto>(sellerProfile.SellerApplication);
        }

        public async Task<UserSellerApplicationPagedDto> GetApplications(SellerApplicationQuery sellerApplicationQuery)
        {
            var sellerApplicationQueryResult =  await _sellerApplicationRepository.Get(sellerApplicationQuery);

            var sellerApplications = sellerApplicationQueryResult.SellerApplications;

            return new UserSellerApplicationPagedDto
            {
                SellerApplications = sellerApplications.Select(sellerApplications => _mapper.Map<UserSellerApplicationDto>(sellerApplications)),
                PageCount = sellerApplicationQueryResult.PageCount
            };
        }

        public async Task<UserBuyersPagedDto> GetBuyers(UserQuery userQuery)
        {
            var buyers = await _userRepository.GetBuyers(userQuery);

            return new UserBuyersPagedDto
            {
                Buyers = buyers.Buyers.Select(buyer => _mapper.Map<UserResponseBuyerDto>(buyer)),
                PageCount = buyers.PageCount
            };
        }

        public async Task<(UserResponseBaseDto? user, UserType? type)> GetById(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return (null, null);
            }

            if (user.UserType == UserType.Buyer)
            {
                var buyerProfile = await _buyerProfileRepository.GetProfileByUserGuid(user.Id);

                if (buyerProfile == null)
                {
                    return (null, null);
                }

                buyerProfile.User = user;

                return (_mapper.Map<UserResponseBuyerDto>(buyerProfile), UserType.Buyer);
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserGuid(user.Id);

                if (sellerProfile == null)
                {
                    return (null, null);
                }

                sellerProfile.User = user;

                return (_mapper.Map<UserResponseSellerDto>(sellerProfile), UserType.Seller);
            }

            if (user.UserType == UserType.Admin)
            {
                return (_mapper.Map<UserResponseAdminDto>(user), UserType.Admin);
            }

            return (null, null);
        }

        public async Task<UserSellersPagedDto> GetSellers(UserQuery userQuery)
        {
            var sellers = await _userRepository.GetSellers(userQuery);

            return new UserSellersPagedDto
            {
                Sellers = sellers.Sellers.Select(seller => _mapper.Map<UserResponseSellerDto>(seller)),
                PageCount = sellers.PageCount
            };
        }

        public async Task<UserStatusDto> GetStatus(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            return await GetStatus(user);
        }

        public async Task<UserStatusDto> GetStatus(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return await GetStatus(user);
        }

        public async Task<UserResponseBaseDto?> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);

            if (user == null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            var loginResult = passwordHasher.VerifyHashedPassword(user, user.Password, userLoginDto.Password);

            if (loginResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            user.LastLoggedIn = DateTime.UtcNow;

            if (user.UserType == UserType.Buyer)
            {
                var buyerProfile = await _buyerProfileRepository.GetProfileByUserGuid(user.Id);

                if (buyerProfile == null)
                {
                    return null;
                }

                buyerProfile.User = user;

                return _mapper.Map<UserResponseBuyerDto>(buyerProfile);
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserGuid(user.Id);

                if (sellerProfile == null)
                {
                    return null;
                }

                sellerProfile.User = user;

                return _mapper.Map<UserResponseSellerDto>(sellerProfile);
            }

            if (user.UserType == UserType.Admin)
            {
                return _mapper.Map<UserResponseAdminDto>(user);
            }

            return null;
        }

        public async Task Logout(Guid userId)
        {
            await _tokenService.RevokeLatestByUserId(userId);
        }

        public async Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);

            if (user != null)
            {
                return null;
            }

            if (userCreateDto is UserCreateBuyerDto buyer)
            {
                return await RegisterBuyer(buyer);
            }

            if (userCreateDto is UserCreateSellerDto seller)
            {
                return await RegisterSeller(seller);
            }

            return null;
        }

        public async Task<UserResponseBaseDto?> Update(Guid userId, UserUpdateBaseDto userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (userUpdateDto is UserUpdateBuyerDto buyer)
            {
                UpdateBuyer(ref user, buyer);

                return _mapper.Map<UserResponseBuyerDto>(user);
            }

            if (userUpdateDto is UserUpdateSellerDto seller)
            {

                UpdateSeller(ref user, seller);

                return _mapper.Map<UserResponseSellerDto>(user);
            }

            return null;
        }

        public async Task<bool> Update(Guid userId, UserStatusUpdateDto userStatusUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var status = Enum.TryParse<UserStatus>(userStatusUpdateDto.Status, true, out var newStatus);

            if (!status)
            {
                return false;
            }

            if (newStatus == UserStatus.OK)
            {
                user.IsBanned = false;

                var subject = "Keepscape Account Unbanned";

                var body = $@"
                <html>
                    <body>
                        <p>Hi {user.FirstName},</p>
                        <p>Your account has been unbanned.</p>
                        <p>Regards,<br />Keepscape Team</p>
                    </body>
                </html>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            if (newStatus == UserStatus.Banned)
            {
                user.IsBanned = true;
                user.StatusReason = userStatusUpdateDto.Reason ?? "";

                var subject = "Keepscape Account Banned";

                // Create a message for banned account
                var body = $@"
                <html>
                    <body>
                        <p>Hi {user.FirstName},</p>
                        <p>Your account has been banned.</p>
                        <p>Reason: {userStatusUpdateDto.Reason}</p>
                        <p>Please contact us if there is a mistake in our decision.</p>
                        <p>Regards,<br />Keepscape Team</p>
                    </body>
                </html>
                ";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UpdateApplication(Guid applicationId, UserSellerApplicationStatusUpdateDto statusUpdate)
        {
            var application = await _sellerApplicationRepository.GetByIdAsync(applicationId);

            if (application == null)
            {
                return false;
            }

            var status = Enum.TryParse<ApplicationStatus>(statusUpdate.Status, true, out var currentStatus);

            if (!status)
            {
                return false;
            }

            application.Status = currentStatus;
            application.StatusReason = statusUpdate.Reason;

            var subject = "Keepscape Seller Application Status Update";

            var body = "";

            if (currentStatus == ApplicationStatus.Approved)
            {
                body = $@"
                <html>
                    <body>
                        <p>Hi {application.SellerProfile!.Name},</p>
                        <p>Your seller application has been approved.</p>
                        <p>Regards,<br />Keepscape Team</p>
                    </body>
                </html>";
            }
            else
            {
                body = $@"
                <html>
                    <body>
                        <p>Hi {application.SellerProfile!.Name},</p>
                        <p>Your seller application has been rejected.</p>
                        <p>Reason: {statusUpdate.Reason}</p>
                        <p>Please contact us if there is a mistake in our decision.</p>
                        <p>Regards,<br />Keepscape Team</p>
                    </body>
                </html>";
            }

            await _emailService.SendEmailAsync(application.SellerProfile.User!.Email, subject, body);

            return await _sellerApplicationRepository.UpdateAsync(application);
        }

        public async Task<bool> UpdatePassword(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, userUpdatePasswordDto.OldPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            user.Password = passwordHasher.HashPassword(user, userUpdatePasswordDto.NewPassword);

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userUpdatePasswordWithCodeDto.Email);

            if (user == null)
            {
                return false;
            }

            var confirmationCodeValid = await _confirmationCodeService.Verify(userUpdatePasswordWithCodeDto.Email, userUpdatePasswordWithCodeDto.Code);

            if (!confirmationCodeValid)
            {
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, userUpdatePasswordWithCodeDto.NewPassword);

            return await _userRepository.UpdateAsync(user);
        }

        private async Task<UserStatusDto> GetStatus(User? user)
        {
            if (user == null)
            {
                return new UserStatusDto
                {
                    UserStatus = UserStatus.NotFound.ToString()
                };
            }

            if (user.IsBanned)
            {
                return new UserStatusDto
                {
                    UserStatus = UserStatus.Banned.ToString(),
                    Reason = user.StatusReason
                };
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserGuid(user.Id);

                if (sellerProfile == null ||
                    sellerProfile.SellerApplication == null ||
                    sellerProfile.SellerApplication.Status == ApplicationStatus.Pending)
                {
                    return new UserStatusDto
                    {
                        UserStatus = UserStatus.Pending.ToString()
                    };
                }

                if (sellerProfile.SellerApplication.Status == ApplicationStatus.Rejected)
                {
                    return new UserStatusDto 
                    { 
                        UserStatus = UserStatus.Rejected.ToString(), 
                        Reason = sellerProfile.SellerApplication.StatusReason
                    };
                }
            }

            return new UserStatusDto { UserStatus = UserStatus.OK.ToString() };
        }
        private async Task<UserResponseBuyerDto> RegisterBuyer(UserCreateBuyerDto userCreateBuyerDto)
        {
            var passwordHasher = new PasswordHasher<User>();

            var newUser = _mapper.Map<User>(userCreateBuyerDto);
            newUser.UserType = UserType.Buyer;
            newUser.Password = passwordHasher.HashPassword(newUser, userCreateBuyerDto.Password);
            
            
            newUser.BuyerProfile = _mapper.Map<BuyerProfile>(userCreateBuyerDto);
            newUser.BuyerProfile.Cart = new Cart
            {
                BuyerProfile = newUser.BuyerProfile
            };

            var createdUser = await _userRepository.AddAsync(newUser);

            return _mapper.Map<UserResponseBuyerDto>(createdUser);
        }

        private async Task<UserResponseSellerDto> RegisterSeller(UserCreateSellerDto userCreateSellerDto)
        {
            var passwordHasher = new PasswordHasher<User>();

            var newUser = _mapper.Map<User>(userCreateSellerDto);
            newUser.UserType = UserType.Seller;
            newUser.Password = passwordHasher.HashPassword(newUser, userCreateSellerDto.Password);
            newUser.Balance = new Balance
            {
                Amount = 0
            };

            newUser.SellerProfile = new SellerProfile
            {
                Name = userCreateSellerDto.SellerName,
                Description = userCreateSellerDto.Description,
                DateTimeCreated = DateTime.Now,
            };

            newUser.SellerProfile.SellerApplication = new SellerApplication
            {
                DateTimeCreated = DateTime.Now,
                IdImageUrl = await _baseImageService.Upload("seller-applications/identification", userCreateSellerDto.IdImage) ?? "",
                BusinessPermitUrl = await _baseImageService.Upload("seller-applications/business-permit", userCreateSellerDto.BusinessPermitImage) ?? "",
                Status = ApplicationStatus.Pending,
                SellerProfile = newUser.SellerProfile
            };

            var createdUser = await _userRepository.AddAsync(newUser);
            return _mapper.Map<UserResponseSellerDto>(createdUser);
        }
        private void UpdateBuyer(ref User user, UserUpdateBuyerDto buyer)
        {
            if (!string.IsNullOrEmpty(buyer.FirstName))
            {
                user.FirstName = buyer.FirstName;
            }
            if (!string.IsNullOrEmpty(buyer.LastName))
            {
                user.LastName = buyer.LastName;
            }
            if (!string.IsNullOrEmpty(buyer.Preferences))
            {
                user.BuyerProfile!.Preferences = buyer.Preferences;
            }
            if (!string.IsNullOrEmpty(buyer.Interests))
            {
                user.BuyerProfile!.Interests = buyer.Interests;
            }
            if (!string.IsNullOrEmpty(buyer.Description))
            {
                user.BuyerProfile!.Description = buyer.Description;
            }
        }

        private void UpdateSeller(ref User user, UserUpdateSellerDto seller)
        {
            if (!string.IsNullOrEmpty(seller.Description))
            {
                user.SellerProfile!.Description = seller.Description;
            }
            if (!string.IsNullOrEmpty(seller.SellerName))
            {
                user.SellerProfile!.Name = seller.SellerName;
            }
        }
    }
}
