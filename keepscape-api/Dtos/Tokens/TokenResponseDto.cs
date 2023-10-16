namespace keepscape_api.Dtos.Tokens
{
    public record TokenResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }
}
