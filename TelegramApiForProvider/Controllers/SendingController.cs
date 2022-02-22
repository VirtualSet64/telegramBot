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
using TelegramApiForProvider.Service;

namespace TelegramApiForProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendingController : ControllerBase
    {
        private OrderContext db;

        private readonly ITelegramBotService _telegramBotService;

        public SendingController(OrderContext context, ITelegramBotService telegramBotService)
        {
            db = context;
            _telegramBotService = telegramBotService;
        }

        Message sentMessage = null;
        ExtendedOrder extendedOrder;

        [HttpPost]
        public async Task/*<ExtendedOrder>*/ ReceiveAndSend(Order order)
        {
            extendedOrder = new ExtendedOrder
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Amount = order.Amount,
                PhoneNumber = order.PhoneNumber,
            };
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                // first row
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Принять", callbackData: $"{extendedOrder.OrderNumber} Принят"),
                    InlineKeyboardButton.WithCallbackData(text: "Отклонить", callbackData: $"{extendedOrder.OrderNumber} Отклонён"),
                });

            string orderText = extendedOrder.OrderNumber + "\n" + extendedOrder.PhoneNumber + "\n" + extendedOrder.Amount;

            if (IsOrderAccept(extendedOrder))
            {
                sentMessage = await _telegramBotService.SendMessage(541041424, orderText, inlineKeyboard);
                extendedOrder.MessageId = sentMessage.MessageId;
                db.ExtendedOrders.Add(extendedOrder);
                await db.SaveChangesAsync();
            }
            //return extendedOrder;
        }

        bool IsOrderAccept(ExtendedOrder extendedOrder)
        {
            if (extendedOrder.MessageId == null)
            {
                return true;
            }
            return false;
        }
    }
}
