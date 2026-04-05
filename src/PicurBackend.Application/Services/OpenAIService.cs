using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;
using System.Text.Json;

namespace PicurBackend.Application.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIClient _client;
        private readonly IReadingRepository _readingRepository;
        private readonly IChatMessageHistoryRepository _chatHistoryRepository;

        public OpenAIService(IConfiguration configuration, IReadingRepository readingRepository, IChatMessageHistoryRepository chatHistoryRepository)
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            _client = new OpenAIClient(apiKey);
            _readingRepository = readingRepository;
            _chatHistoryRepository = chatHistoryRepository;
        }

        public async Task<string> AskAI(string question)
        {
            var chatClient = _client.GetChatClient("gpt-4.1-mini");

            var history = await _chatHistoryRepository.GetHistoryAsync();

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("""
                    Eres un experto en monitoreo de cadena de frío
                    para neveras de vacunas.

                    Analiza:
                    - temperatura
                    - aperturas de puerta
                    - riesgo sanitario
                    - anomalías
                    - pérdida de cadena de frío

                    Mantén el contexto conversacional de las preguntas anteriores.
                    Cuando necesites datos históricos usa la herramienta disponible.
                """)
            };

            foreach (var item in history.OrderBy(x => x.CreatedAt))
            {
                if (item.Role == "user")
                    messages.Add(new UserChatMessage(item.Content));
                else if (item.Role == "assistant")
                    messages.Add(new AssistantChatMessage(item.Content));
            }

            messages.Add(new UserChatMessage(question));


            var getReadingsTool = ChatTool.CreateFunctionTool(
                "get_readings_by_range",
                "Obtiene lecturas por rango de fechas",
                BinaryData.FromString(DateRangeSchema.Json)
            );

            var averageTool = ChatTool.CreateFunctionTool(
                "get_average_temperature",
                "Calcula temperatura promedio por rango",
                BinaryData.FromString(DateRangeSchema.Json)
            );

            var doorTool = ChatTool.CreateFunctionTool(
                "get_door_events",
                "Obtiene eventos donde la puerta estuvo abierta",
                BinaryData.FromString(DateRangeSchema.Json)
            );

            var anomalyTool = ChatTool.CreateFunctionTool(
                "get_anomalies",
                "Obtiene lecturas fuera del rango seguro entre 2°C y 8°C",
                BinaryData.FromString(DateRangeSchema.Json)
            );

            var options = new ChatCompletionOptions();
            options.Tools.Add(getReadingsTool);
            options.Tools.Add(averageTool);
            options.Tools.Add(doorTool);
            options.Tools.Add(anomalyTool);

            await _chatHistoryRepository.SaveMessageAsync(new ChatMessageHistory
            {
                Role = "user",
                Content = question,
                CreatedAt = DateTime.UtcNow
            });

            var firstResponse = await chatClient.CompleteChatAsync(messages, options);

            var toolCall = firstResponse.Value.ToolCalls.FirstOrDefault();
            string toolResult = "";

            if (toolCall != null)
            {
                var args = JsonDocument.Parse(toolCall.FunctionArguments);

                var startDate = args.RootElement.GetProperty("startDate").GetDateTime();
                var endDate = args.RootElement.GetProperty("endDate").GetDateTime();

                Console.WriteLine(startDate.ToString());
                Console.WriteLine(endDate.ToString());

                switch (toolCall.FunctionName)
                {
                    case "get_readings_by_range":
                        var readings = await _readingRepository
                            .GetByDateRangeAsync(startDate, endDate);

                        toolResult = string.Join("\n", readings.Select(r =>
                            $"Temp:{r.Temperature}, Door:{r.Door}, Time:{r.Timestamp}"
                        ));
                        break;

                    case "get_average_temperature":
                        var avg = await _readingRepository
                            .GetAverageTemperatureAsync(startDate, endDate);

                        toolResult = $"Temperatura promedio: {avg}";
                        break;

                    case "get_door_events":
                        var doors = await _readingRepository
                            .GetDoorEventsAsync(startDate, endDate);

                        toolResult = $"Cantidad de aperturas: {doors.Count()}";
                        break;

                    case "get_anomalies":
                        var anomalies = await _readingRepository
                            .GetAnomaliesAsync(startDate, endDate);

                        toolResult = anomalies.Any()
                            ? string.Join("\n", anomalies.Select(a =>
                                $"Temp:{a.Temperature}, Time:{a.Timestamp}"
                            ))
                            : "No se detectaron anomalías en el rango consultado.";
                        break;
                }
                messages.Add(new AssistantChatMessage(firstResponse.Value));
                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));

                var finalResponse = await chatClient.CompleteChatAsync(messages);

                var finalText = finalResponse.Value.Content.FirstOrDefault()?.Text
                    ?? "No hubo respuesta final.";

                await _chatHistoryRepository.SaveMessageAsync(new ChatMessageHistory
                {
                    Role = "assistant",
                    Content = finalText,
                    CreatedAt = DateTime.UtcNow
                });

                return finalText;
            }

            var responseText = firstResponse.Value.Content.FirstOrDefault()?.Text
                ?? "No hubo respuesta.";

            await _chatHistoryRepository.SaveMessageAsync(new ChatMessageHistory
            {
                Role = "assistant",
                Content = responseText,
                CreatedAt = DateTime.UtcNow
            });

            return responseText;
        }
    }
}