using keepscape_api.Configurations;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;


namespace keepscape_api.Services.OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIAPI _client;
        private readonly OpenAIConfig _openAIConfig;

        public OpenAIService(IOptions<OpenAIConfig> openAIConfig)
        {
            _openAIConfig = openAIConfig.Value;
            _client = new OpenAIAPI(_openAIConfig.Key);
        }

        public async Task<string?> Prompt(string? prompt)
        {
            var result = await _client.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 1,
                Messages = new ChatMessage[]
                {
                    new ChatMessage(ChatMessageRole.System, prompt)
                }
            });

            return result.Choices[0].Message.Content;
        }
    }
}
