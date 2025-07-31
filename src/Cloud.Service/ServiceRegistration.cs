using Application.Interfaces.CloudService.AWS;
using Application.Interfaces.CloudService.Google;
using Application.Interfaces.Service.Email;
using Application.Interfaces.Service.Invoice;
using Cloud.Service.AWS;
using Cloud.Service.Email;
using Cloud.Service.Google;
using Cloud.Service.Invoice;
using Microsoft.Extensions.DependencyInjection;

namespace Cloud.Service
{
    public static class ServiceRegistration
    {
        public static void AddCloudServiceInfrastructure(this IServiceCollection services, Func<string, bool> writeLogMethod)
        {
            writeLogMethod("[SYSTEM INIT] [STARTING] Cloud service initialization.");

            services.AddTransient<IAwsS3Service, AwsS3Service>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailTemplateProvider, EmailTemplateProvider>();
            services.AddTransient<IGoogleApiIdentityToolkitCloudService, GoogleApiIdentityToolkitCloudService>();
            services.AddTransient<IInvoiceProvider, InvoiceProvider>();
            services.AddTransient<IHandleInvoiceService, HandleInvoiceService>();
            services.AddTransient<ICurrencyProvider, CurrencyProvider>();

            writeLogMethod("[SYSTEM INIT] [SUCCESS] Cloud service initialization.");
        }
    }
}