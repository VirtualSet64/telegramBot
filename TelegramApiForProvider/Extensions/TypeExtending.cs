using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Extensions
{
    public static class TypeExtending
    {
        public static string AsString(this OrderPaymentMethods paymentMethod)
        {
            return paymentMethod switch
            {
                OrderPaymentMethods.Cash => "Наличными",
                OrderPaymentMethods.Online => "Онлайн",
                _ => "Не определен."
            };
        }
    }
}
