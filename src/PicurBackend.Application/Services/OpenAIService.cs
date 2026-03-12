using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Application.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIClient _client;
        private readonly IReadingRepository _readingRepository;

        public OpenAIService(IConfiguration configuration, IReadingRepository readingRepository)
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            _client = new OpenAIClient(apiKey);
            _readingRepository = readingRepository;
        }

        public async Task<string> AskAI(string prompt)
        {
            var chatClient = _client.GetChatClient("gpt-4.1-mini");

            var readings = await _readingRepository.GetAllAsync();

            var context = string.Join("\n", readings.Select(r =>
                $"Temp:{r.Temperature} Door:{r.Door} Time:{r.Timestamp}"
            ));

             prompt = $"""
                Analiza estos datos de sensores de una nevera de vacunas.

                Datos:
                {context}

                {prompt}
                """;

            var response = await chatClient.CompleteChatAsync(
                new UserChatMessage(prompt)
            );

            return response.Value.Content[0].Text;
        }
    }
}