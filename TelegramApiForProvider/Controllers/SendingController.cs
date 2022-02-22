using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Extended;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private OrderContext db;

        public SendingController(OrderContext context, IConfiguration configuration)
        {
            db = context;
            _configuration = configuration;
        }

        Message sentMessage = null;
        ExtendedOrder extendedOrder;

        [HttpPost]
        public async Task ReceiveAndSend(Order order)
        {
            TelegramBotClient botClient = new TelegramBotClient($"{_configuration["Token"]}");

            var deserializeOrder = JsonConvert.DeserializeObject<Order>(order.ToString());
            extendedOrder = new ExtendedOrder
            {
                Id = deserializeOrder.Id,
                OrderNumber = deserializeOrder.OrderNumber,
                Amount = deserializeOrder.Amount,
                PhoneNumber = deserializeOrder.PhoneNumber,
            };
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                // first row
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Принять", callbackData: $"{extendedOrder.OrderNumber} Принят"),
                    InlineKeyboardButton.WithCallbackData(text: "Отклонить", callbackData: $"{extendedOrder.OrderNumber} Отклонён"),
                });

            string orderText = extendedOrder.OrderNumber + "\n" + extendedOrder.PhoneNumber + "\n" + extendedOrder.Amount;

            if (extendedOrder.IsAccept == null && extendedOrder.MessageId == null)
            {
                sentMessage = await botClient.SendTextMessageAsync(
                        chatId: 541041424,
                        text: orderText,
                        replyMarkup: inlineKeyboard
                        );
                extendedOrder.MessageId = sentMessage.MessageId;
                db.ExtendedOrders.Add(extendedOrder);
                await db.SaveChangesAsync();
            }
        }
    }
}
