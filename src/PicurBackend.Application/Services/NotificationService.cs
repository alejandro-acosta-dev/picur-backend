using Microsoft.Extensions.Configuration;
using PicurBackend.Application.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PicurBackend.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(
                configuration["Twilio:AccountSid"],
                configuration["Twilio:AuthToken"]
            );
        }

        public async Task<string> SendSmsAsync()
        {
            var fromPhoneNumber = _configuration["Twilio:FromPhoneNumberWhatsapp"];
            var toPhoneNumber = _configuration["Twilio:ToPhoneNumber"];

            var code = new Random().Next(100000, 999999);

            await MessageResource.CreateAsync(
                body: $"Su código de recuperación de contraseña es: {code}",
                from: new PhoneNumber(fromPhoneNumber),
                to: new PhoneNumber($"whatsapp:{toPhoneNumber}")
            );
            return code.ToString();
        }
    }
}
