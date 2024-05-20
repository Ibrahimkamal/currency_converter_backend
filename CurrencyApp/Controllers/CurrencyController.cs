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

                if (ExcludedCurrencies.Contains(from) || ExcludedCurrencies.Contains(to))
                {
                    return BadRequest(new { error = $"Conversion involving {from} or {to} is not allowed" });
                }
                
                if (!AllowedCurrencies.Contains(from) || !AllowedCurrencies.Contains(to))
                {
                    return BadRequest(new { error = $"Currency {from} or {to} is not supported" });
                }

                double result = await _currencyService.ConvertCurrency(from.ToUpper(), to.ToUpper(), amount);
                return Ok(new { from, to, amount, result });
            }
            catch (ValueProviderException ex)
            {
                return BadRequest(new { error = ex.Message });
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
