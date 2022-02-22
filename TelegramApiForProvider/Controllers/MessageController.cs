using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Extended;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private OrderContext db;
        public MessageController(OrderContext context, IConfiguration configuration)
        {
            db = context;
            _configuration = configuration;
        }
        Models.User user = new Models.User();

        long chatId = 0;

        [HttpPost]
        public async Task Update([FromBody] object obj)
        {
            TelegramBotClient botClient = new TelegramBotClient($"{_configuration["Token"]}");

            Update update = JsonConvert.DeserializeObject<Update>(obj.ToString());

            if (update.Message != null)
            {
                chatId = update.Message.Chat.Id;
                user.ChatId = (int)chatId;
                db.Users.Add(user);
                db.SaveChanges();
            }
            if (update.CallbackQuery != null)
            {
                List<Models.User> users = db.Users.ToList();
                List<ExtendedOrder> extendedOrders = db.ExtendedOrders.ToList();
                foreach (var user in users)
                {
                    foreach (var item in extendedOrders)
                    {
                        if (item.IsAccept == null)
                        {
                            if (update.CallbackQuery.Data == $"{item.OrderNumber} Принят")
                            {
                                item.IsAccept = true;
                                await botClient.SendTextMessageAsync(
                                    chatId: user.ChatId,
                                    text: $"Заказ номер {item.OrderNumber} принят на обработку"
                                );

                                await botClient.EditMessageReplyMarkupAsync(
                                    chatId: user.ChatId,
                                    messageId: (int)item.MessageId
                                    );

                                RequestData requestData = new RequestData
                                {
                                    OrderId = item.Id,
                                    StatusId = (int)OrderStatus.Accept
                                };
                                await SendStatusAsync(requestData);
                            }
                            if (update.CallbackQuery.Data == $"{item.OrderNumber} Отклонён")
                            {
                                item.IsAccept = false;
                                await botClient.SendTextMessageAsync(
                                    chatId: user.ChatId,
                                    text: $"Заказ номер {item.OrderNumber} отклонён"
                                );
                                await botClient.EditMessageReplyMarkupAsync(
                                    chatId: user.ChatId,
                                    messageId: (int)item.MessageId
                                );

                                RequestData requestData = new RequestData
                                {
                                    OrderId = item.Id,
                                    StatusId = (int)OrderStatus.Cancel
                                };
                                await SendStatusAsync(requestData);
                            }
                        }
                    }
                }
            }
        }
        public async Task SendStatusAsync(RequestData requestData)
        {
            var json = JsonConvert.SerializeObject(requestData, Formatting.Indented,
                                    new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://staging-api.crondostav.ru/api/adminpanel/v1/Tbot/SetOrderStatus";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);
        }
    }
}
