using System.Threading.Tasks;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Service
{
    public interface ISendService
    {
        Task SendStatus(RequestData requestData);
        Task<bool> ConfirmPassword(string phoneNumber);
    }
}
