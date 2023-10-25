namespace keepscape_api.Services.OpenAI
{
    public interface IOpenAIService
    {
        public Task<string?> Prompt(string? prompt);
    }
}
