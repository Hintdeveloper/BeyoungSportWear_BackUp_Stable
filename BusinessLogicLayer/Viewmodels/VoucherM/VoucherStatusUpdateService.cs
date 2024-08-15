using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class VoucherStatusUpdateService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VoucherStatusUpdateService> _logger;

    public VoucherStatusUpdateService(IHttpClientFactory httpClientFactory, ILogger<VoucherStatusUpdateService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateVoucherStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating voucher status.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Delay 1 giây
        }
    }
    public class CheckVoucherCodeResponse
    {
        public bool Exists { get; set; }
    }
    private async Task UpdateVoucherStatusAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://localhost:7241/api/VoucherM/update-status", null);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Voucher statuses updated successfully.");
        }
        else
        {
            _logger.LogError("Failed to update voucher statuses.");
        }
    }
}
