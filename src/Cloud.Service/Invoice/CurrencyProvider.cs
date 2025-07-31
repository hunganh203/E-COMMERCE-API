using Application.Dtos;
using Application.Interfaces.Service.Invoice;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Cloud.Service.Invoice
{
    internal class CurrencyProvider : ICurrencyProvider
    {
        private readonly ILogger<CurrencyProvider> _logger;

        public CurrencyProvider(ILogger<CurrencyProvider> logger)
        {
            _logger = logger;
        }

        public async Task<List<CurrencyExchangeRateDto>> GetAllExchangeRate()
        {
            var uri = "";
            try
            {
                using var client = new HttpClient
                {
                    BaseAddress = new Uri("https://portal.vietcombank.com.vn")
                };

                var result = await client.GetAsync($"/Usercontrols/TVPortal.TyGia/pXML.aspx?b=8");

                var resultContent = await result.Content.ReadAsStreamAsync();

                var serializer = new XmlSerializer(typeof(ExrateList));
                var exrateList = (ExrateList)serializer.Deserialize(resultContent)!;

                return exrateList.Exrates.Select(x => new CurrencyExchangeRateDto
                {
                    CurrencyCode = x.CurrencyCode,
                    CurrencyName = x.CurrencyName,
                    Buy = double.TryParse(x.Buy, out var buy) ? buy : 0,
                    Sell = double.TryParse(x.Sell, out var sell) ? sell : 0,
                    Transfer = double.TryParse(x.Transfer, out var transfer) ? transfer : 0,
                }).ToList();

                //var errMsg = response.error.message;
                //var errMsgVal = $"Can not send verification: {errMsg.Value}";
                //throw new Exception(errMsgVal);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Can not GetAllExchangeRate ");
                _logger.LogError($"Result from {uri}");
                if (e.Message.Contains("Can not send verification"))
                {
                    throw new Exception(e.Message);
                }
                throw new Exception($"Can not send verification code GetAllExchangeRate");
            }
        }

        public async Task<CurrencyExchangeRateDto> GetExchangeRateByCode(string code)
        {
            var listExchangeRate = await GetAllExchangeRate();
            var exchangeRate = listExchangeRate.FirstOrDefault(x => x.CurrencyCode == code);
            if (exchangeRate == null)
                throw new Exception("CURRENCY_CODE_DOES_NOT_SUPPORT");
            return exchangeRate!;
        }
    }
}