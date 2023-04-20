using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Models;
using TelegramApiForProvider.Service;

namespace TelegramApiForProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendingController : ControllerBase
    {
        private readonly OrderContext db;

        private readonly ITelegramBotService _telegramBotService;
        private readonly IOrderService _orderService;
        private readonly ISendService _sendService;

        public SendingController(OrderContext context, ITelegramBotService telegramBotService,
            IOrderService orderService, ISendService sendService)
        {
            db = context;
            _telegramBotService = telegramBotService;
            _orderService = orderService;
            _sendService = sendService;
        }

        [HttpPost]
        public async Task ReceiveAndSend(OrderParameter orderParameter)
        {
            try
            {
                var user = db.Users.Where(x => x.PartnerId == orderParameter.PartnerId).FirstOrDefault();
                var response = _sendService.ConfirmPassword(user.PhoneNumber).Result;
                if (response != null)
                {
                    var users = db.Users.ToList();
                    //var users = db.Users.Where(x => x.PartnerId == orderParameter.PartnerId).ToList();
                    Order order = new Order
                    {
                        Id = orderParameter.Id,
                        OrderNumber = orderParameter.OrderNumber,
                        PartnerName = orderParameter.PartnerName,
                        PartnerId = orderParameter.PartnerId,
                        //CreateDatetime = orderParameter.CreateDatetime
                    };
                    foreach (var item in users)
                    {
                        OrderMessage orderMessage = new OrderMessage
                        {
                            Id = Guid.NewGuid(),
                            MessageId = null,
                            ChatId = item.ChatId,
                            OrderId = orderParameter.Id,
                            Order = order
                        };
                        db.OrderMessages.Add(orderMessage);
                    }
                    db.Orders.Add(order);
                    await db.SaveChangesAsync();

                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                        new InlineKeyboardButton[]
                        {
                    InlineKeyboardButton.WithCallbackData(text: "Принять ✅", callbackData: $"{orderParameter.OrderNumber} Принят"),
                    InlineKeyboardButton.WithCallbackData(text: "Отклонить ❌", callbackData: $"{orderParameter.OrderNumber} Отклонён"),
                        });

                    string orderText = "";

                    orderText = _orderService.CreateDescriptionForCron(orderParameter);

                    //if (orderParameter.DeliveryType.Id == (int)DeliveryName.CronMarket)
                    //    {
                    //        orderText = _orderService.CreateDescriptionForCron(orderParameter);
                    //    }
                    //    if (orderParameter.DeliveryType.Id == (int)DeliveryName.Marketplace)
                    //    {
                    //        orderText = _orderService.CreateDescriptionForPartner(orderParameter);
                    //    }

                    foreach (var item in users)
                    {
                        var orderrr = db.OrderMessages.FirstOrDefault(x => x.MessageId == null && x.OrderId == orderParameter.Id);
                        try
                        {
                            var sentMessage = await _telegramBotService.SendMessage(orderrr.ChatId, orderText, inlineKeyboard, ParseMode.Html);
                            orderrr.MessageId = sentMessage.MessageId;
                            db.OrderMessages.Update(orderrr);
                        }
                        catch (Exception ex)
                        {
                            db.OrderMessages.Remove(orderrr);
                            throw;
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
