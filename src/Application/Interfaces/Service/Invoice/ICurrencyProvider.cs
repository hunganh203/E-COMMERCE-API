using Application.Dtos;

namespace Application.Interfaces.Service.Invoice
{
    public interface ICurrencyProvider
    {
        Task<List<CurrencyExchangeRateDto>> GetAllExchangeRate();

        Task<CurrencyExchangeRateDto> GetExchangeRateByCode(string code);
    }
}