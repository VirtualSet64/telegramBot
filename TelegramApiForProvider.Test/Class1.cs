using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using TelegramApiForProvider.Controllers;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Models;
using TelegramApiForProvider.Service;
using Xunit;

namespace TelegramApiForProvider.Tests
{
    public class Class1
    {
        
        //[Fact]
        //public void IndexViewDataMessage()
        //{
        //    // Arrange
        //    var mockTelegramBotService = new Mock<ITelegramBotService>();
        //    mockTelegramBotService.Setup(service => service.SendMessage(123, "Text"));

        //    var mockOrderContext = new Mock<OrderContext>();
        //    mockOrderContext.Setup(context => context.SaveChanges());
        //    var controller = new SendingController(mockOrderContext.Object, mockTelegramBotService.Object);
        //    //// Act
        //    TypeExtending result = null;// controller.ReceiveAndSend(order);

        //    //// Assert
        //    Assert.NotNull(result);
        //}
    }
}
