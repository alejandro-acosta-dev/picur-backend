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
                    Eres un experto en monitoreo de cadena de frío para neveras de vacunas.

                    Analiza:
                    - temperatura (rango seguro: 2°C – 8°C)
                    - aperturas de puerta
                    - riesgo sanitario
                    - anomalías
                    - pérdida de cadena de frío

                    IMPORTANTE sobre fechas y herramientas:
                    - Todos los timestamps en la base de datos están en UTC.
                    - El usuario enviará su fecha y hora local entre corchetes al inicio de cada mensaje, por ejemplo: [Fecha y hora actual: jueves, 15 de mayo de 2026, 09:51].
                    - Cuando el usuario diga "hoy", "ahora", "última hora", etc., usa esa fecha local para calcular el rango UTC correspondiente (resta el offset de zona horaria si lo conoces, o usa un rango amplio ±1 día para no perder datos).
                    - Siempre usa las herramientas disponibles para consultar datos reales antes de responder preguntas sobre temperaturas o eventos.
                    - Si una herramienta no devuelve datos, intenta con un rango más amplio antes de concluir que no hay registros.
                    - Mantén el contexto conversacional de los mensajes anteriores.
                    - Responde siempre en español, de forma clara y directa, sin mostrar JSON ni metadatos técnicos.
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

            if (firstResponse.Value.ToolCalls.Count > 0)
            {
                messages.Add(new AssistantChatMessage(firstResponse.Value));

                foreach (var toolCall in firstResponse.Value.ToolCalls)
                {
                    string toolResult = "";

                    try
                    {
                        var args = JsonDocument.Parse(toolCall.FunctionArguments);
                        var startDate = args.RootElement.GetProperty("startDate").GetDateTime();
                        var endDate = args.RootElement.GetProperty("endDate").GetDateTime();

                        switch (toolCall.FunctionName)
                        {
                            case "get_readings_by_range":
                                var readings = await _readingRepository
                                    .GetByDateRangeAsync(startDate, endDate);
                                toolResult = readings.Any()
                                    ? string.Join("\n", readings.Select(r =>
                                        $"Temp:{r.Temperature}, Door:{r.Door}, Time:{r.Timestamp}"))
                                    : "No hay lecturas en ese rango.";
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
                                        $"Temp:{a.Temperature}, Time:{a.Timestamp}"))
                                    : "No se detectaron anomalías en el rango consultado.";
                                break;

                            default:
                                toolResult = "Herramienta no reconocida.";
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        toolResult = $"Error al ejecutar la herramienta: {ex.Message}";
                    }

                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                }

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