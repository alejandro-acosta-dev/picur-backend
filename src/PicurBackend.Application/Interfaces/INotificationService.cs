namespace PicurBackend.Application.Interfaces
{
    public interface INotificationService
    {
        Task<string> SendSmsAsync();
    }
}
