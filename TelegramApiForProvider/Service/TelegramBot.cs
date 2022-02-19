using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramApiForProvider.Service
{
    public class TelegramBot
    {
        private readonly IConfiguration _configuration;
        public TelegramBot(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TelegramBotClient> GetTelegramBot()
        {
            var telegramBot = new TelegramBotClient($"{_configuration["Token"]}");// 5289996800:AAEjRMLLvhmA67f - hBUTKUQ1v43qEi7kOFQ");

            var hook = $"{_configuration["Url"]}api/message/update"; //"https://6d2c-2a00-1fa1-41a9-73b7-34f5-515f-1819-91bd.ngrok.io/api/message/update";
            await telegramBot.SetWebhookAsync(hook);

            return telegramBot;
        }
    }
}
