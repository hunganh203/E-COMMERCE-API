using Application.Interfaces.Service;
using Application.Interfaces.Service.Email;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;
using Shared.Services.System;

namespace Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, Func<string, bool> writeLogMethod)
        {
            writeLogMethod("[SYSTEM INIT] [STARTING] Shared Infrastructure initialization.");

            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenRefreshService, TokenRefreshService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddTransient<IAttributeService, AttributeService>();
            services.AddTransient<IVerificationService, VerificationService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderDetailService, OrderDetailService>();

            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IWebsiteService, WebsiteService>();

            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IReviewService, ReviewService>();

            services.AddTransient<IProductDisplayService, ProductDisplayService>();
            services.AddTransient<ISetupService, SetupService>();
            services.AddTransient<IRoleService, RoleService>();
        }
    }
}