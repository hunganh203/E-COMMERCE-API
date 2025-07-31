using Application.Interfaces.Repositories.EfCore;
using EfCore.Persistence.Contexts;
using EfCore.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EfCore.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddEfCorePersistenceInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Func<string, bool> writeLogMethod)
        {
            writeLogMethod("[SYSTEM INIT] [STARTING] EFCore persistence initialization.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            });

            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITokenRefreshRepository, TokenRefreshRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IProductImageRepository, ProductImageRepository>();
            services.AddTransient<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddTransient<IProductRelatedRepository, ProductRelatedRepository>();
            services.AddTransient<IAttributeRepository, AttributeRepository>();
            services.AddTransient<IUserVerificationRepository, UserVerificationRepository>();

            services.AddTransient<IOrderDetailRepository, OrderDetailRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IWebsiteRepository, WebsiteRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();

            services.AddTransient<IProductDisplayRepository, ProductDisplayRepository>();

            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            services.AddTransient<IUserClaimRepository, UserClaimRepository>();
        }
    }
}