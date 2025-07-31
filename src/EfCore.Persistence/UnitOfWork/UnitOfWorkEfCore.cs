using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Repositories.EFCore;
using EfCore.Persistence.Contexts;
using EfCore.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EfCore.Persistence.UnitOfWork
{
    public class UnitOfWorkEfCore : IUnitOfWorkEfCore
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWorkEfCore(ApplicationDbContext context)
        {
            _context = context;

            Users = new UserRepository(_context);
            TokenRefresh = new TokenRefreshRepository(_context);
            Articles = new ArticleRepository(_context);
            Attributes = new AttributeRepository(_context);
            Customers = new CustomerRepository(_context);
            EmailConfigurations = new EmailConfigurationRepository(_context);
            EmailRegistrations = new EmailRegistrationRepository(_context);
            EmailSignUps = new EmailSignUpRepository(_context);
            EmailTemplates = new EmailTemplateRepository(_context);
            Galleries = new GalleryRepository(_context);
            Menus = new MenuRepository(_context);
            Orders = new OrderRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            Products = new ProductRepository(_context);
            ProductAttributes = new ProductAttributeRepository(_context);
            ProductImages = new ProductImageRepository(_context);
            ProductRelateds = new ProductRelatedRepository(_context);
            Reviews = new ReviewRepository(_context);
            Websites = new WebsiteRepository(_context);
            UserVerifications = new UserVerificationRepository(_context);
            Roles = new RoleRepository(_context);
            UserRoles = new UserRoleRepository(_context);
            UserClaims = new UserClaimRepository(_context);
        }

        public IUserRepository Users { get; }
        public ITokenRefreshRepository TokenRefresh { get; }
        public IArticleRepository Articles { get; }
        public IAttributeRepository Attributes { get; set; }
        public ICustomerRepository Customers { get; set; }
        public IEmailConfigurationRepository EmailConfigurations { get; set; }
        public IEmailRegistrationRepository EmailRegistrations { get; set; }
        public IEmailSignUpRepository EmailSignUps { get; set; }
        public IEmailTemplateRepository EmailTemplates { get; set; }
        public IGalleryRepository Galleries { get; set; }
        public IMenuRepository Menus { get; set; }
        public IOrderRepository Orders { get; set; }
        public IOrderDetailRepository OrderDetails { get; set; }
        public IProductRepository Products { get; set; }
        public IProductAttributeRepository ProductAttributes { get; set; }
        public IProductImageRepository ProductImages { get; set; }
        public IProductRelatedRepository ProductRelateds { get; set; }
        public IReviewRepository Reviews { get; set; }
        public IWebsiteRepository Websites { get; set; }
        public IUserVerificationRepository UserVerifications { get; set; }
        public IRoleRepository Roles { get; set; }
        public IUserRoleRepository UserRoles { get; set; }
        public IUserClaimRepository UserClaims { get; set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}