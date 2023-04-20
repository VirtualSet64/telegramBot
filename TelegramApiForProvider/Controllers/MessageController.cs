using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramApiForProvider.AccountService;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Models;
using TelegramApiForProvider.Service;
using TelegramApiForProvider.ViewModels;

namespace TelegramApiForProvider.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly OrderContext db;

        private readonly RegisterService _registerService;

        private readonly ITelegramBotService _telegramBotService;
        private readonly ISendService _sendService;

        public MessageController(OrderContext context, ITelegramBotService telegramBotService, ISendService sendService,
            RegisterService registerService)
        {
            db = context;
            _telegramBotService = telegramBotService;
            _sendService = sendService;
            _registerService = registerService;
        }

        [HttpPost]
        public async Task Update([FromBody] Update update)
        {
            try
            {
                if (update.Message != null)
                {
                    if (update.Message.Text == "/start")
                    {
                        _telegramBotService.SendMessage(update.Message.From.Id, "Введите номер телефона в формате 79*********");
                    }
                    else
                    {
                        var phoneNumber = update.Message.Text;
                        //var response = _sendService.ConfirmPassword(phoneNumber).Result;
                        //if (response != null)
                        //{
                        if (!db.Users.Any(x => x.ChatId == update.Message.From.Id))
                        {
                            Models.User user = new Models.User
                            {
                                Name = update.Message.From.Username,
                                PhoneNumber = update.Message.Text,
                                ChatId = update.Message.From.Id,
                                //PartnerId = response.PartnerId
                            };

                            UserForIdentity userForIdentity = new UserForIdentity
                            {
                                PhoneNumber = phoneNumber,
                            };

                            AccountViewModel registerViewModel = new AccountViewModel()
                            {
                                PhoneNumber = phoneNumber,
                                Password = "123"
                            };
                            await _registerService.Register(registerViewModel, userForIdentity);

                            db.Users.Add(user);
                            await db.SaveChangesAsync();
                            //_telegramBotService.SendMessage(update.Message.From.Id, $"Вход выполнен, {response.PartnerName}, ждите заказов");
                        }
                        else
                        {
                            Models.User userUpdate = db.Users.FirstOrDefault(n => n.PhoneNumber == phoneNumber);
                            //userUpdate.PartnerId = response.PartnerId;
                            userUpdate.Name = phoneNumber;
                            userUpdate.PhoneNumber = phoneNumber;
                            //db.Users.Update(userUpdate);
                            //await db.SaveChangesAsync();
                            //_telegramBotService.SendMessage(update.Message.From.Id, $"Вход выполнен, {response.PartnerName}, ждите заказов");
                        }
                        //}
                        //else
                        //{
                        //    _telegramBotService.SendMessage(update.Message.From.Id, "Ошибка при входе");
                        //}
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
            await CallbackHandlingAsync(update.CallbackQuery);
        }

        public async Task CallbackHandlingAsync(CallbackQuery callbackQuery)
        {
            try
            {
                if (callbackQuery != null)
                {
                    long chatId = callbackQuery.From.Id;
                    List<OrderMessage> orderMessages = db.OrderMessages
                        .Include(x => x.Order)
                        .Where(x => x.ChatId == chatId &&
                                    x.Order.IsAccept == null &&
                                    x.MessageId != null)
                        .ToList();

                    foreach (var order in orderMessages)
                    {
                        if (callbackQuery.Data == $"{order.Order.OrderNumber} Принят")
                        {
                            string orderNumber = callbackQuery.Data.Replace(" Принят", "");

                            order.Order.IsAccept = true;
                            await _telegramBotService.SendMessage(order.ChatId, $"Заказ номер {order.Order.OrderNumber} принят на обработку.", order.MessageId);
                            _telegramBotService.EditMessage(order.ChatId, (int)order.MessageId);

                            RequestData requestData = new RequestData
                            {
                                OrderId = order.Order.Id,
                                StatusId = (int)OrderStatus.Accept
                            };
                            //await _sendService.SendStatus(requestData);
                        }
                        if (callbackQuery.Data == $"{order.Order.OrderNumber} Отклонён")
                        {
                            string orderNumber = callbackQuery.Data.Replace(" Отклонён", "");

                            order.Order.IsAccept = false;
                            await _telegramBotService.SendMessage(order.ChatId, $"Заказ номер {order.Order.OrderNumber} отклонён.", order.MessageId);
                            _telegramBotService.EditMessage(order.ChatId, (int)order.MessageId);

                            RequestData requestData = new RequestData
                            {
                                OrderId = order.Order.Id,
                                StatusId = (int)OrderStatus.Cancel
                            };
                            //await _sendService.SendStatus(requestData);
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
