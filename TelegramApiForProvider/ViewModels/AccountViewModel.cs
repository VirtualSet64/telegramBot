using System.ComponentModel.DataAnnotations;

namespace TelegramApiForProvider.ViewModels
{
    public class AccountViewModel
    {
        [Required]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
