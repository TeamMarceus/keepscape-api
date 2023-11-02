using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.ConfirmationCodes;
using keepscape_api.Services.Emails;
using keepscape_api.Services.OpenAI;
using keepscape_api.Services.Products;
using keepscape_api.Services.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace keepscape_api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ISellerApplicationRepository _sellerApplicationRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _baseImageService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IOpenAIService _openAIService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository,          
            IBuyerProfileRepository buyerProfileRepository, 
            ISellerProfileRepository sellerProfileRepository, 
            ISellerApplicationRepository sellerApplicationRepository,
            ICategoryRepository categoryRepository,
            IImageService baseImageService,
            ITokenService tokenService,
            IEmailService emailService,
            IConfirmationCodeService confirmationCodeService,
            IOpenAIService openAIService,
            IProductService productService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _sellerApplicationRepository = sellerApplicationRepository;
            _categoryRepository = categoryRepository;
            _baseImageService = baseImageService;
            _tokenService = tokenService;
            _emailService = emailService;
            _confirmationCodeService = confirmationCodeService;
            _openAIService = openAIService;
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<UserSellerApplicationDto?> GetApplication(Guid userId)
        {
            var sellerProfile = await _sellerProfileRepository.GetProfileByUserId(userId);

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
            var (Buyers, PageCount) = await _userRepository.GetBuyers(userQuery);

            return new UserBuyersPagedDto
            {
                Buyers = Buyers.Select(buyer => _mapper.Map<UserResponseBuyerDto>(buyer)),
                PageCount = PageCount
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
                var buyerProfile = await _buyerProfileRepository.GetProfileByUserId(user.Id);

                if (buyerProfile == null)
                {
                    return (null, null);
                }

                buyerProfile.User = user;

                return (_mapper.Map<UserResponseBuyerDto>(buyerProfile), UserType.Buyer);
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserId(user.Id);

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
            var (Sellers, PageCount) = await _userRepository.GetSellers(userQuery);

            return new UserSellersPagedDto
            {
                Sellers = Sellers.Select(seller => _mapper.Map<UserResponseSellerDto>(seller)),
                PageCount = PageCount
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
                var buyerProfile = await _buyerProfileRepository.GetProfileByUserId(user.Id);

                if (buyerProfile == null)
                {
                    return null;
                }

                buyerProfile.User = user;

                return _mapper.Map<UserResponseBuyerDto>(buyerProfile);
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserId(user.Id);

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
            }

            if (userUpdateDto is UserUpdateSellerDto seller)
            {

                UpdateSeller(ref user, seller);
            }

            await _userRepository.UpdateAsync(user);

            return user.UserType == UserType.Buyer ? _mapper.Map<UserResponseBuyerDto>(user) : _mapper.Map<UserResponseSellerDto>(user);
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
            if (passwordVerificationResult == PasswordVerificationResult.Success && userUpdatePasswordDto.NewPassword == userUpdatePasswordDto.OldPassword)
            {
                return false;
            }
            if (userUpdatePasswordDto.NewPassword != userUpdatePasswordDto.ConfirmNewPassword)
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
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserId(user.Id);

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
            
            var buyerProfile = await _buyerProfileRepository.GetProfileByUserId(createdUser.Id);

            createdUser.BuyerProfile = buyerProfile;

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
            if (!string.IsNullOrEmpty(buyer.PhoneNumber))
            {
                user.PhoneNumber = buyer.PhoneNumber;
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
            if (!string.IsNullOrEmpty(buyer.DeliveryAddress))
            {
                user.BuyerProfile!.DeliveryAddress = buyer.DeliveryAddress;
            }
            if (!string.IsNullOrEmpty(buyer.DeliveryFullName))
            {
                user.BuyerProfile!.DeliveryFullName = buyer.DeliveryFullName;
            }
            if (!string.IsNullOrEmpty(buyer.AltMobileNumber))
            {
                user.BuyerProfile!.AltMobileNumber = buyer.AltMobileNumber;
            }
        }

        private void UpdateSeller(ref User user, UserUpdateSellerDto seller)
        {
            if (!string.IsNullOrEmpty(seller.Description))
            {
                user.SellerProfile!.Description = seller.Description;
            }
            if (!string.IsNullOrEmpty(seller.PhoneNumber))
            {
                user.PhoneNumber = seller.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(seller.SellerName))
            {
                user.SellerProfile!.Name = seller.SellerName;
            }
        }

        public async Task<bool> CreateBuyerSuggestions(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (user.UserType != UserType.Buyer)
            {
                return false;
            }

            var categories = await _categoryRepository.GetAllAsync();

            var promptForAI = $"Given the following categories and buyer attitude:\nCategories:\n{string.Join("\n", categories.Select(x => x.Name))}"+
                $"\nDescription: {user.BuyerProfile!.Description}, Interests: {user.BuyerProfile!.Interests}, Preferences: {user.BuyerProfile!.Preferences}\n"+
                "Choose strictly the best category that the buyer will most likely love in the list above. Rank the top 3 and explain why in the format:\n"+
                "\nAddress the description as if you're talking with the person. Connect the description to the nice things in Region 7 in the Philippines.\n"+
                "[START][1] CategoryName : Description [ENDS HERE]\n" +
                "[START][2] CategoryName : Description [ENDS HERE]\n" +
                "[START][3] CategoryName : Description [ENDS HERE]\n"
                ;

            var response = await _openAIService.Prompt(promptForAI);

            if (response == null)
            {
                return false;
            }

            var categoriesWithDescription = response.Contains("[ENDS HERE]") ? response.Split("[ENDS HERE]") : response.Split("\n");

            var categoriesDescriptionDictionary = new Dictionary<string, string>();

            foreach(var categoryWithDescription in categoriesWithDescription)
            {
                var categoryWithDescriptionCleaned = categoryWithDescription.Trim().Replace("\n", "");
                var categoryWithDescriptionSplit = categoryWithDescriptionCleaned.Split(":");

                if (categoryWithDescriptionSplit.Length != 2)
                {
                    continue;
                }

                var categoryName = categoryWithDescriptionSplit[0].Replace("[START]", "").Trim();
                var regex = new Regex(@"\[[1-3]\]");
                categoryName= regex.Replace(categoryName, "", 1).Trim();
                var categoryDescription = categoryWithDescriptionSplit[1].Trim();

                if (string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(categoryDescription))
                {
                    continue;
                }
                if (categoriesDescriptionDictionary.Any(x => x.Key == categoryName))
                {
                    continue;
                }
                if (!categories.Any(x => x.Name == categoryName))
                {
                    continue;
                }

                categoriesDescriptionDictionary[categoryName] = categoryDescription;
            }

            var buyerProfile = await _buyerProfileRepository.GetProfileByUserId(user.Id);

            if (buyerProfile == null)
            {
                return false;
            }

            if (buyerProfile.BuyerCategoryPreferences == null)
            {
                buyerProfile.BuyerCategoryPreferences = new List<BuyerCategoryPreference>();
            }
            else
            {
                buyerProfile.BuyerCategoryPreferences.Clear();
            }

            foreach (var categoryDescription in categoriesDescriptionDictionary)
            {
                var category = categories.FirstOrDefault(x => x.Name == categoryDescription.Key);

                if (category == null)
                {
                    continue;
                }

                var buyerSuggestion = new BuyerCategoryPreference
                {
                    CategoryId = category.Id,
                    Description = categoryDescription.Value
                };

                if (!buyerProfile.BuyerCategoryPreferences.Any(x => x.CategoryId == category.Id))
                {
                    buyerProfile.BuyerCategoryPreferences.Add(buyerSuggestion);
                }
            }

            await _buyerProfileRepository.UpdateAsync(buyerProfile);

            return true;
        }

        public async Task<IEnumerable<UserBuyerSuggestionsDto>> GetBuyerSuggestions(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Enumerable.Empty<UserBuyerSuggestionsDto>();
            }

            if (user.UserType != UserType.Buyer)
            {
                return Enumerable.Empty<UserBuyerSuggestionsDto>();
            }

            var buyerProfile = await _buyerProfileRepository.GetProfileByUserId(user.Id);

            if (buyerProfile == null)
            {
                return Enumerable.Empty<UserBuyerSuggestionsDto>();
            }

            if (buyerProfile.BuyerCategoryPreferences == null)
            {
                var created = await CreateBuyerSuggestions(userId);

                if (!created)
                {
                    return Enumerable.Empty<UserBuyerSuggestionsDto>();
                }

                buyerProfile = await _buyerProfileRepository.GetProfileByUserId(user.Id);

                if (buyerProfile == null)
                {
                    return Enumerable.Empty<UserBuyerSuggestionsDto>();
                }

                if (buyerProfile.BuyerCategoryPreferences == null)
                {
                    return Enumerable.Empty<UserBuyerSuggestionsDto>();
                }
            }

            var categories = await _categoryRepository.GetAllAsync();

            var buyerSuggestions = new List<UserBuyerSuggestionsDto>();

            foreach (var buyerCategoryPreference in buyerProfile.BuyerCategoryPreferences)
            {
                var category = categories.FirstOrDefault(x => x.Id == buyerCategoryPreference.CategoryId);

                if (category == null)
                {
                    continue;
                }

                var products = await _productService.Get(new ProductQuery
                {
                    Categories = new List<string>() { category.Name }, 
                    Page = 1,
                    PageSize = 15
                }
                );

                buyerSuggestions.Add(new UserBuyerSuggestionsDto
                {
                    Category = category.Name,
                    Description = buyerCategoryPreference.Description,
                    Products = products.Products.Select(p => _mapper.Map<ProductResponseHomeDto>(p))
                });
            }

            return buyerSuggestions;
        }
    }
}
