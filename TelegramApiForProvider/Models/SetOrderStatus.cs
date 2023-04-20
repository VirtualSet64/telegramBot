using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramApiForProvider.Models
{
    public class SetOrderStatus
    {
        public Guid OrderId { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
