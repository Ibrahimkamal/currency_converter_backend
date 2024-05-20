using CurrencyApp.Services;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace CurrencyApp.Tests.Services;

public class CurrencyServiceTests
{
    [Fact]
    public void ConvertCurrency_WhenCalled_ReturnsConvertedAmount()
    {
        // Arrange
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<CurrencyService>>();
        var currencyService = new CurrencyService(httpClientFactory.Object, logger.Object);
        string from = "USD";
        string to = "EUR";
        double amount = 100;
        double expected = 85.0;
        
        // Act
        double result = currencyService.ConvertCurrency(from, to, amount).Result;
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GetLatest_WhenCalled_ReturnsLatestRates()
    {
        // Arrange
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<CurrencyService>>();
        var currencyService = new CurrencyService(httpClientFactory.Object, logger.Object);
        string baseCurrency = "USD";
        
        // Act
        var result = currencyService.GetLatest(baseCurrency).Result;
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetHistoricalRatesTimeSeries_WhenCalled_ReturnsHistoricalRates()
    {
        // Arrange
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<CurrencyService>>();
        var currencyService = new CurrencyService(httpClientFactory.Object, logger.Object);
        string baseCurrency = "USD";
        DateTime startDate = new DateTime(2022, 1, 1);
        DateTime endDate = new DateTime(2022, 1, 31);
        int pageIndex = 1;
        int pageSize = 10;

        // Act
        var result = currencyService.GetHistoricalRatesTimeSeries(baseCurrency, startDate, endDate, pageIndex, pageSize)
            .Result;

        // Assert
        Assert.NotNull(result);
    }
}