using CloudinaryDotNet;
using System.Security.Principal;

namespace TweetClone.API.Services.Implementation
{
    public static class ClodinaryServiceExtension
    {
        public static void AddCloudinary(this IServiceCollection services, Account account)
        {
            services.AddSingleton(new Cloudinary(account));
        }

        public static Account GetAccount(IConfiguration configuration)
        {
            var cloudName = configuration.GetValue<string>("AccountSettings:CloudName");
            var apiKey = configuration.GetValue<string>("AccountSettings:ApiKey");
            var apiSecret = configuration.GetValue<string>("AccountSettings:ApiSecret");

            if (new[] { cloudName, apiKey, apiSecret }.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Please specify Cloudinary account details!");
            }

            return new Account(cloudName, apiKey, apiSecret);
        }
    }
}
