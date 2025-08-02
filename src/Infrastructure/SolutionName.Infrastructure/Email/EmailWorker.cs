using SolutionName.Application.Abstractions.Services;
using SolutionName.Infrastructure.Email.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace SolutionName.Infrastructure.Email
{
    public class EmailWorker : BackgroundService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailQueue _emailQueue;
        private readonly ILogger<EmailWorker> _logger;
        private readonly TimeSpan _pollInterval;

        public EmailWorker(
            IEmailSender emailSender,
            IEmailQueue emailQueue,
            ILogger<EmailWorker> logger,
            IOptions<EmailWorkerSettings> workerSettings)
        {
            _emailSender = emailSender;
            _emailQueue = emailQueue;
            _logger = logger;
            _pollInterval = TimeSpan.FromSeconds(workerSettings.Value.PollIntervalSeconds);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var emails = new List<CompiledEmailMessage>();
                CompiledEmailMessage email;
                // Dequeue all available emails
                while ((email = await _emailQueue.DequeueEmailAsync()) != null)
                {
                    emails.Add(email);
                }

                if (emails.Count > 0)
                {
                    var sw = Stopwatch.StartNew();
                    _logger.LogInformation("Sending {Count} emails...", emails.Count);
                    var sendTasks = emails.Select(async msg =>
                    {
                        try
                        {
                            await _emailSender.SendAsync(msg);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", msg.To.Select(t => t.Address)));
                        }
                    });
                    await Task.WhenAll(sendTasks);
                    sw.Stop();
                    _logger.LogInformation("Sent {Count} emails in {ElapsedMs} ms", emails.Count, sw.ElapsedMilliseconds);
                }

                // Wait before polling again
                await Task.Delay(_pollInterval, stoppingToken);
            }
        }
    }
}
