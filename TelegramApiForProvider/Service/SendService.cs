using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Service
{
    public class SendService : ISendService
    {
        public async Task<bool> ConfirmPassword(string phoneNumber)
        {
            var url = "https://staging-api.crondostav.ru/api/adminpanel/v1/Tbot/CheckPartnerPhone?" + $"partnerPhone={phoneNumber}";
            using var client = new HttpClient();

            var response = await client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task SendStatus(RequestData requestData)
        {
            var json = JsonConvert.SerializeObject(requestData, Formatting.Indented,
                                    new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://staging-api.crondostav.ru/api/adminpanel/v1/Tbot/SetOrderStatus";
            using var client = new HttpClient();

            var response = await client.PutAsync(url, data);
        }
    }
}
