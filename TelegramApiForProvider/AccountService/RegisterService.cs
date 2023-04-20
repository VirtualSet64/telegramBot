using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TelegramApiForProvider.Models;
using TelegramApiForProvider.ViewModels;

namespace TelegramApiForProvider.AccountService
{
    public class RegisterService
    {
        private readonly UserManager<UserForIdentity> _userManager;
        private readonly SignInManager<UserForIdentity> _signInManager;

        public RegisterService(UserManager<UserForIdentity> userManager, SignInManager<UserForIdentity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> Register(AccountViewModel model, UserForIdentity user)
        {
            user.UserName = user.PhoneNumber;
            // добавляем пользователя
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // установка куки
                await _signInManager.SignInAsync(user, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Login(AccountViewModel model)
        {
            try
            {
                var result =
            await _signInManager.PasswordSignInAsync(model.PhoneNumber, model.Password, false, false);
                return result.Succeeded;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
