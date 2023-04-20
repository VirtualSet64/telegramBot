﻿using System;
using System.Text;
using TelegramApiForProvider.Extensions;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Service
{
    public class OrderService : IOrderService
    {
        public string CreateDescriptionForCron(OrderParameter orderParameter)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<b>Новый заказ.</b>");
            message.AppendLine();
            //message.AppendLine($"Заказ № {orderParameter.OrderNumber} от {orderParameter.CreateDatetime:G}");
            message.AppendLine();

            //foreach (var dish in orderParameter.Products)
            //{
            //    message.AppendLine($"{dish.Name}. Кол-во: {dish.Quantity}");
            //    if (dish.Additives.Count != 0)
            //    {
            //        message.AppendLine("Добавки:");
            //        foreach (var additive in dish.Additives)
            //        {
            //            message.AppendLine($"{additive.Name}");
            //        }
            //    }

            //    message.AppendLine();
            //}
            //message.AppendLine($"Адрес доставки: {orderParameter.DeliveryAddress}");
            //message.AppendLine();
            //message.AppendLine($"Комментарий: {orderParameter.Comment}");
            message.AppendLine();
            message.AppendLine($"Партнер: {orderParameter.PartnerName}");
            message.AppendLine();
            //message.AppendLine($"Кол-во персон: {orderParameter.CutleryQuantity}");
            //message.AppendLine();
            //message.AppendLine($"Способ оплаты: {orderParameter.PaymentMethod.AsString()}");
            //message.AppendLine();
            //message.AppendLine($"Сумма заказа: {orderParameter.TotalAmount}");

            return message.ToString();
        }

        public string CreateDescriptionForPartner(OrderParameter orderParameter)
        {
            //var deliverDatetime = orderParameter.DeliverAtDatetime.HasValue ?
            //    orderParameter.DeliverAtDatetime.Value.Date == DateTime.Today ?
            //    $"Сегодня {orderParameter.DeliverAtDatetime.Value.ToString("HH:mm")}" :
            //    orderParameter.DeliverAtDatetime.Value.Date == DateTime.Today + TimeSpan.FromDays(1) ?
            //    $"Завтра {orderParameter.DeliverAtDatetime.Value.ToString("HH:mm")}" :
            //    $"{orderParameter.DeliverAtDatetime.Value.ToString("dd.MM HH:mm")}" :
            //    "Как можно скорее";

            StringBuilder message = new StringBuilder();
            message.AppendLine("<b>Новый заказ.</b>");
            message.AppendLine();
            //message.AppendLine($"Заказ № {orderParameter.OrderNumber} от {orderParameter.CreateDatetime:G}");
            message.AppendLine();

            //foreach (var dish in orderParameter.Products)
            //{
            //    message.AppendLine($"{dish.Name}. Кол-во: {dish.Quantity}");
            //    if (dish.Additives.Count != 0)
            //    {
            //        message.AppendLine("Добавки:");
            //        foreach (var additive in dish.Additives)
            //        {
            //            message.AppendLine($"{additive.Name}");
            //        }
            //    }

            //    message.AppendLine();
            //}
            //message.AppendLine($"Адрес доставки: {orderParameter.DeliveryAddress}");
            message.AppendLine();
            //message.AppendLine($"Комментарий: {orderParameter.Comment}");
            message.AppendLine();
            message.AppendLine($"Партнер: {orderParameter.PartnerName}");
            message.AppendLine();
            //message.AppendLine($"Кол-во персон: {orderParameter.CutleryQuantity}");
            //message.AppendLine();
            //message.AppendLine($"Доставить к: {deliverDatetime}");
            //message.AppendLine();
            //message.AppendLine($"Телефон: {orderParameter.PhoneNumber}");
            message.AppendLine();
            message.AppendLine($"Имя: {orderParameter.CustomerName}");
            message.AppendLine();
            //message.AppendLine($"Способ оплаты: {orderParameter.PaymentMethod.AsString()}");
            message.AppendLine();
            //message.AppendLine($"Сумма заказа: {orderParameter.TotalAmount}");

            return message.ToString();
        }
    }
}
