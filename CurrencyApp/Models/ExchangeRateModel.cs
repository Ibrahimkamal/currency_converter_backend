namespace CurrencyApp.Models;
public class ExchangeRateModel
{
    public double Amount { get; set; } = 0.0;
    public string Base { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.MinValue;
    public Dictionary<string, double> Rates { get; set; } = new Dictionary<string, double>();
}
public class TimeSeriesExchangeRateModel
{
    public double Amount { get; set; } = 0.0;
    public string Base { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.MinValue;

    public Dictionary<DateTime, Dictionary<string, double>> Rates { get; set; } =
        new Dictionary<DateTime, Dictionary<string, double>>();
}