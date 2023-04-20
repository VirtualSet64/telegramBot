using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TelegramApiForProvider.Extensions;
using TelegramApiForProvider.Models;

namespace TelegramApiForProvider.DbService
{
    public class OrderContext : IdentityDbContext<UserForIdentity>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderMessage> OrderMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserForIdentity> UsersForIdentity { get; set; }


        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public OrderContext() : base()
        {

        }
    }
}
