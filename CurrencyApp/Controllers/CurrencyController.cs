using CurrencyApp.Models;
using CurrencyApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CurrencyApp.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController(CurrencyService currencyService) : ControllerBase
    {
        private readonly CurrencyService _currencyService = currencyService;
        private static readonly HashSet<string> ExcludedCurrencies =new HashSet<string> { "TRY", "PLN", "THB", "MXN" };
        private static readonly HashSet<string> AllowedCurrencies =  new HashSet<string>
        {
            "AUD", "BGN", "BRL", "CAD", "CHF", "CNY", "CZK", "DKK", "GBP", "HKD",
            "HUF", "IDR", "ILS", "INR", "ISK", "JPY", "KRW", "MXN", "MYR", "NOK",
            "NZD", "PHP", "PLN", "RON", "SEK", "SGD", "THB", "TRY", "USD", "ZAR"
        };

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] double amount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || amount <= 0)
                {
                    return BadRequest(new { error = "Missing or invalid 'from', 'to', or 'amount' parameter" });
                }

                from = from.ToUpper();
                to = to.ToUpper();
                if (ExcludedCurrencies.Contains(from) || ExcludedCurrencies.Contains(to))
                {
                    return BadRequest(new { error = $"Conversion involving {from} or {to} is not allowed" });
                }
                
                if (!AllowedCurrencies.Contains(from) || !AllowedCurrencies.Contains(to))
                {
                    return BadRequest(new { error = $"Currency {from} or {to} is not supported" });
                }

                double result = await _currencyService.ConvertCurrency(from, to, amount);
                return Ok(new { from, to, amount, result });
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest([FromQuery] string baseCurrency)
        {
            try
            {
                baseCurrency = baseCurrency.ToUpper();
                if (string.IsNullOrWhiteSpace(baseCurrency) || !AllowedCurrencies.Contains(baseCurrency))
                {
                    return BadRequest(new { error = "Missing or invalid 'base' parameter" });
                }
                ExchangeRateModel result = await _currencyService.GetLatest(baseCurrency);
                return Ok(new {result.Base, result.Rates});
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
