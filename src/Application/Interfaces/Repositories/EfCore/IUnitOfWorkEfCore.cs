using Application.Interfaces.Repositories.EfCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces.Repositories.EFCore
{
    public interface IUnitOfWorkEfCore : IDisposable
    {
        IUserRepository Users { get; }
        IArticleRepository Articles { get; }
        IAttributeRepository Attributes { get; set; }
        ICustomerRepository Customers { get; set; }
        IEmailConfigurationRepository EmailConfigurations { get; set; }
        IEmailRegistrationRepository EmailRegistrations { get; set; }
        IEmailSignUpRepository EmailSignUps { get; set; }
        IEmailTemplateRepository EmailTemplates { get; set; }
        IGalleryRepository Galleries { get; set; }
        IMenuRepository Menus { get; set; }
        IOrderRepository Orders { get; set; }
        IOrderDetailRepository OrderDetails { get; set; }
        IProductRepository Products { get; set; }
        IProductAttributeRepository ProductAttributes { get; set; }
        IProductImageRepository ProductImages { get; set; }
        IProductRelatedRepository ProductRelateds { get; set; }
        IReviewRepository Reviews { get; set; }
        IWebsiteRepository Websites { get; set; }
        IUserVerificationRepository UserVerifications { get; set; }

        int Complete();

        IDbContextTransaction BeginTransaction();
    }
}