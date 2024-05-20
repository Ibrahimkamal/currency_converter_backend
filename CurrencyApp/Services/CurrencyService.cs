using CurrencyApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            long utcNow = GetCet();
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(utcNow).UtcDateTime;
            if (!IsAfter16Cet())
            {
                date = date.AddDays(-1); // If it's before 16:00 CET, we need to get the rates for the previous day
            }
            Dictionary<string, double> rates = await GetCorrespondingRates(date.Date, from);
            if (!rates.ContainsKey(to))
            {
                _logger.LogError($"No exchange rate found for {to} on {date:yyyy-MM-dd}");
                throw new ValueProviderException($"No exchange rate found for {to} on {date:yyyy-MM-dd}");
            }
            double rate = rates[to];
            return amount * rate;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private async Task<Dictionary<string, double>> GetCorrespondingRates(DateTime date, string currency)
    {
        try
        {
            if (_currencyDatabase.ContainsKey(date) && _currencyDatabase[date].ContainsKey(currency))
            {
                return _currencyDatabase[date][currency].Rates;
            }

            var response = await _httpClientFactory.CreateClient().GetAsync($"https://api.frankfurter.app/{date:yyyy-MM-dd}?base={currency}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                throw;
            }
            var content = await response.Content.ReadAsStringAsync();
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

            return exchangeRateModel.Rates;
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