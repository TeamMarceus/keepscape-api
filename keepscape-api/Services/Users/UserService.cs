using AutoMapper;
using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Generics;
using keepscape_api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace keepscape_api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository<BuyerProfile> _buyerProfileRepository;
        private readonly IProfileRepository<SellerProfile> _sellerProfileRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IProfileRepository<BuyerProfile> buyerProfileRepository, IProfileRepository<SellerProfile> sellerProfileRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _mapper = mapper;
        }

        public async Task<UserStatus> GetStatus(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return UserStatus.NotFound;
            }

            if (user.IsBanned)
            {
                return UserStatus.Banned;
            }

            if (user.UserType == UserType.Seller)
            {
                var sellerProfile = await _sellerProfileRepository.GetProfileByUserGuid(user.Id);

                if (sellerProfile == null || 
                    sellerProfile.SellerApplication == null ||
                    sellerProfile.SellerApplication.Status == ApplicationStatus.Pending)
                {
                    return UserStatus.Pending;
                }

                if (sellerProfile.SellerApplication.Status == ApplicationStatus.Rejected)
                {
                    return UserStatus.Banned;
                }
            }

            return UserStatus.OK;
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

        public async Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);

            if (user != null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            if (userCreateDto is UserCreateBuyerDto buyer)
            {
                var newUser = _mapper.Map<User>(buyer);
                newUser.UserType = UserType.Buyer;
                newUser.BuyerProfile = _mapper.Map<BuyerProfile>(buyer);
                newUser.Password = passwordHasher.HashPassword(newUser, buyer.Password);

                var createdUser = await _userRepository.AddAsync(newUser);

                if (createdUser == null)
                {
                    return null;
                }

                return _mapper.Map<UserResponseBuyerDto>(createdUser);
            }

            if (userCreateDto is UserCreateSellerDto seller)
            {
                var newUser = _mapper.Map<User>(seller);
                newUser.UserType = UserType.Seller;
                newUser.SellerProfile = _mapper.Map<SellerProfile>(seller);
                newUser.Password = passwordHasher.HashPassword(newUser, seller.Password);

                var createdUser = await _userRepository.AddAsync(newUser);

                if (createdUser == null)
                {
                    return null;
                }

                return _mapper.Map<UserResponseSellerDto>(createdUser);
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
                if (user.BuyerProfile == null)
                {
                    return null;
                }

                UpdateBuyer(ref user, buyer);

                return _mapper.Map<UserResponseBuyerDto>(user);
            }

            if (userUpdateDto is UserUpdateSellerDto seller)
            {
                if (user.SellerProfile == null)
                {
                    return null;
                }

                UpdateSeller(ref user, seller);

                return _mapper.Map<UserResponseSellerDto>(user);
            }

            return null;
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

        public Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto)
        {
            throw new NotImplementedException();
        }

        private void UpdateBuyer(ref User user, UserUpdateBuyerDto buyer)
        {
            if (buyer.FirstName != null)
            {
                user.FirstName = buyer.FirstName;
            }
            if (buyer.LastName != null)
            {
                user.LastName = buyer.LastName;
            }
            if (buyer.Preferences != null)
            {
                user.BuyerProfile!.Preferences = buyer.Preferences;
            }
            if (buyer.Interests != null)
            {
                user.BuyerProfile!.Interests = buyer.Interests;
            }
            if (buyer.Description != null)
            {
                user.BuyerProfile!.Description = buyer.Description;
            }
        }

        private void UpdateSeller(ref User user, UserUpdateSellerDto seller)
        {
            if (seller.Description != null)
            {
                user.SellerProfile!.Description = seller.Description;
            }
            if (seller.SellerName != null)
            {
                user.SellerProfile!.Name = seller.SellerName;
            }
        }
    }
}
