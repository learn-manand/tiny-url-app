using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class CleanupFunction
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseApiUrl;

    public CleanupFunction(ILoggerFactory loggerFactory, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = loggerFactory.CreateLogger<CleanupFunction>();
        _httpClient = httpClient;
        _baseApiUrl = configuration["BaseApiUrl"]; // reads from settings
    }

    [Function("CleanupFunction")]
    public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation("Cleanup function triggered at: {time}", DateTimeOffset.Now);

        var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/api/delete-all");
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("All URLs deleted successfully.");
        }
        else
        {
            _logger.LogWarning("Failed to delete URLs. Status: {status}", response.StatusCode);
        }
    }
}