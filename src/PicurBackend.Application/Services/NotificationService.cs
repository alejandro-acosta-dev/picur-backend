using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PicurBackend.Application.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration) {
            this._configuration = configuration;

            var accountSid = configuration["Twilio:AccountSid"];
            var authToken = configuration["Twilio:AuthToken"];

            TwilioClient.Init(accountSid, authToken);
        }
        public async Task<string> SendSmsAsync()
        {
            var fromPhoneNumber = _configuration["Twilio:FromPhoneNumberWhatsapp"];
            var toPhoneNumber = _configuration["Twilio:ToPhoneNumber"];

            var random = new Random();
            int code = random.Next(100000, 999999);

            var msg = await MessageResource.CreateAsync(
                body: $"Su código de recuperación de contraseña es: {code}",
                from: new PhoneNumber(fromPhoneNumber),
                to: new PhoneNumber($"whatsapp:{toPhoneNumber}")
            );
                
            return code.ToString();
        }
    }
}