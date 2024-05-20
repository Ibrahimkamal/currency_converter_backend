using CurrencyApp.Models;
using Newtonsoft.Json;

namespace CurrencyApp.Services;
public class CurrencyService(IHttpClientFactory httpClientFactory, ILogger<CurrencyService> logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<CurrencyService> _logger = logger;

    private readonly Dictionary<DateTime, Dictionary<string, ExchangeRateModel>> _currencyDatabase = new Dictionary<DateTime, Dictionary<string, ExchangeRateModel>>();

    public async Task<double> ConvertCurrency(string from, string to, double amount)
    {
        try
        {
            if (from == to)
            {
                return amount;
            }
            long utcNow = GetCet();
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(utcNow).UtcDateTime;
            if (!IsAfter16Cet())
            {
                date = date.AddDays(-1); // If it's before 16:00 CET, we need to get the rates for the previous day
            }
            ExchangeRateModel exchangeRate = await GetCorrespondingRates(date.Date, from);
            double rate = exchangeRate.Rates[to];
            return amount * rate;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }
    
    public async Task<ExchangeRateModel> GetLatest(string baseCurrency)
    {
        try
        {
            long utcNow = GetCet();
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(utcNow).UtcDateTime;
            if (!IsAfter16Cet())
            {
                date = date.AddDays(-1); // If it's before 16:00 CET, we need to get the rates for the previous day
            }
            return await GetCorrespondingRates(date.Date, baseCurrency);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }
    private async Task<ExchangeRateModel> GetCorrespondingRates(DateTime date, string currency)
    {
        try
        {
            if (_currencyDatabase.ContainsKey(date) && _currencyDatabase[date].ContainsKey(currency))
            {
                return _currencyDatabase[date][currency];
            }
            string endpoint = $"https://api.frankfurter.app/{date:yyyy-MM-dd}?base={currency}";
            var response = await _httpClientFactory.CreateClient().GetAsync(endpoint);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                throw;
            }
            string content = await response.Content.ReadAsStringAsync();
            var exchangeRateModel = JsonConvert.DeserializeObject<ExchangeRateModel>(content);

            if (!_currencyDatabase.ContainsKey(date))
            {
                _currencyDatabase[date] = new Dictionary<string, ExchangeRateModel>();
            }
            if (exchangeRateModel==null)
            {
                _logger.LogError($"No exchange rate found for {currency} on {date:yyyy-MM-dd}");
                throw new Exception($"No exchange rate found for {currency} on {date:yyyy-MM-dd}");
            }
            _currencyDatabase[date][currency] = exchangeRateModel;

            return exchangeRateModel;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }


    private bool IsAfter16Cet()
    {
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;
        TimeZoneInfo cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTimeOffset nowCet = TimeZoneInfo.ConvertTime(nowUtc, cetTimeZone);
        DateTimeOffset sixteenCet = new DateTimeOffset(nowCet.Year, nowCet.Month, nowCet.Day, 16, 0, 0, cetTimeZone.BaseUtcOffset);

        return nowCet >= sixteenCet;
    }
    private long GetCet()
    {
        // Get the current UTC time
        DateTimeOffset now = DateTimeOffset.UtcNow;

        // Define the Central European Time zone
        TimeZoneInfo cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        // Convert UTC time to CET
        DateTimeOffset cetTime = TimeZoneInfo.ConvertTime(now, cetTimeZone);

        // Return the CET time as a Unix timestamp
        return cetTime.ToUnixTimeSeconds();
    }
}