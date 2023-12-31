﻿using keepscape_api.Dtos.Tokens;

namespace keepscape_api.Services.Tokens
{
    public interface ITokenService
    {
        Task<TokenResponseDto?> Create(TokenCreateDto tokenCreateDto);
        Task<bool> Verify(TokenVerifyDto tokenVerifyDto);
        Task<TokenResponseDto?> Refresh(TokenRefreshDto tokenRefreshDto);
        Task<bool> Revoke(string refreshToken);
        Task RevokeLatestByUserId(Guid userId);
    }
}
