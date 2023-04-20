using System.Threading.Tasks;
using TelegramApiForProvider.Contract;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.Service
{
    public interface ISendService
    {
        Task SendStatus(RequestData requestData);
        Task<CheckPartnerResponseModel> ConfirmPassword(string phoneNumber);
    }
}
