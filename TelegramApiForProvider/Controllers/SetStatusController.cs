using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("api/[controller]")]
    [ApiController]
    public class SetStatusController : Controller
    {
        private readonly OrderContext db;

        private readonly ITelegramBotService _telegramBotService;

        private readonly UserManager<Models.UserForIdentity> _userManager;
        private readonly SignInManager<Models.UserForIdentity> _signInManager;

        private readonly RegisterService _registerService;

        public SetStatusController(OrderContext context, ITelegramBotService telegramBotService, UserManager<Models.UserForIdentity> userManager, 
            SignInManager<Models.UserForIdentity> signInManager, RegisterService registerService)
        {
            db = context;
            _telegramBotService = telegramBotService;
            _userManager = userManager;
            _signInManager = signInManager;
            _registerService = registerService;
        }

        [HttpPost]
        public async Task SetOrderStatus(SetOrderStatus setOrderStatus)
        {
            try
            {
                AccountViewModel loginViewModel = new AccountViewModel()
                {
                    PhoneNumber = setOrderStatus.PhoneNumber,
                    Password = setOrderStatus.Password
                };

                if (await _registerService.Login(loginViewModel))
                {
                    Order order = db.Orders.FirstOrDefault(c => c.Id == setOrderStatus.OrderId && c.IsAccept == null);
                    if (order != null)
                    {
                        List<OrderMessage> orderMessages = db.OrderMessages
                                .Include(x => x.Order)
                                .Where(x => x.Order.Id == setOrderStatus.OrderId).ToList();

                        foreach (var item in orderMessages)
                        {
                            if (setOrderStatus.StatusId == 2)
                            {
                                order.IsAccept = true;
                                await _telegramBotService.SendMessage(item.ChatId, $"Заказ номер {item.Order.OrderNumber} принят на обработку.", item.MessageId);
                                _telegramBotService.EditMessage(item.ChatId, (int)item.MessageId);
                            }
                            else if (setOrderStatus.StatusId == 5)
                            {
                                order.IsAccept = false;
                                await _telegramBotService.SendMessage(item.ChatId, $"Заказ номер {item.Order.OrderNumber} отклонён.", item.MessageId);
                                _telegramBotService.EditMessage(item.ChatId, (int)item.MessageId);
                            }
                        }
                        db.Orders.Update(order);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
