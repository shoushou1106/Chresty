using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chresty.Core.Services;
sealed class LoggerService : BackgroundService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger) =>
        _logger = logger;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Logger running at: {time}", DateTimeOffset.UtcNow);
            await Task.Delay(1_000, stoppingToken);
        }
    }
}