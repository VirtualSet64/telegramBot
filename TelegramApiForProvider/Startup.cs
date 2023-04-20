using Coravel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramApiForProvider.AccountService;
using TelegramApiForProvider.DbService;
using TelegramApiForProvider.Models;
using TelegramApiForProvider.Service;

namespace TelegramApiForProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<OrderContext>(options =>
                options.UseSqlServer(connection, optionsRetry => optionsRetry.EnableRetryOnFailure()));

            services.AddIdentity<UserForIdentity, IdentityRole>(
                opts => {
                    opts.Password.RequiredLength = 2;   // минимальная длина
                    opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
                    opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
                    opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
                    opts.Password.RequireDigit = false; // требуются ли цифры
                })
                .AddEntityFrameworkStores<OrderContext>();

            services.AddScheduler();
            services.AddTransient<ReminderService>();

            services.AddTransient<RegisterService>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<ISendService, SendService>(); 

            services.AddSingleton<ITelegramBotService>(provider =>
            {
                return new TelegramBotService(Configuration["Token"], Configuration["Url"]);
            });

            Configurations.AdminPanel = Configuration["AdminPanel"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices.UseScheduler(scheduler => scheduler.Schedule<ReminderService>().Cron("*/1 * * * *"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
