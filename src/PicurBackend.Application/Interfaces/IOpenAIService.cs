namespace PicurBackend.Application.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> AskAI(string prompt);
    }
}
