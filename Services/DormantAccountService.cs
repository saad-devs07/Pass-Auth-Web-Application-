//using Microsoft.AspNetCore.Identity;
//using PassAuthWebApp_.Areas.Identity.Data;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.CodeAnalysis.Elfie.Diagnostics;

//namespace PassAuthWebApp_.Services
//{
//    public class DormantAccountService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<DormantAccountService> _logger;

//        public DormantAccountService(IServiceProvider serviceProvider, ILogger<DormantAccountService> logger)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                await CheckDormantAccounts();
//                //await Task.Delay(TimeSpan.FromDays(1), stoppingToken);  // Run daily
//                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);  // Run every 5 minutes
//            }
//        }

//        private async Task CheckDormantAccounts()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//                var users = userManager.Users.ToList();

//                foreach (var user in users)
//                {
//                    var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

//                    if (isAdmin)
//                    {
//                        _logger.LogInformation($"Skipping dormancy check for Admin: {user.UserName}");
//                        continue;
//                    }

//                    if (!user.IsDormant && user.LastLoginDate.AddDays(90) < DateTime.UtcNow)
//                    {
//                        user.IsDormant = true;
//                        await userManager.UpdateAsync(user);
//                    }
//                }
//            }
//        }
//    }
//}
