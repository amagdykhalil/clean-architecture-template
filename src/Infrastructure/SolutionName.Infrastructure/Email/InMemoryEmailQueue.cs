using SolutionName.Application.Abstractions.Services;
using System.Collections.Concurrent;

namespace SolutionName.Infrastructure.Email
{
    public class InMemoryEmailQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<CompiledEmailMessage> _queue = new ConcurrentQueue<CompiledEmailMessage>();

        public Task EnqueueEmailAsync(CompiledEmailMessage message)
        {
            _queue.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task<CompiledEmailMessage> DequeueEmailAsync()
        {
            _queue.TryDequeue(out var message);
            return Task.FromResult(message);
        }
    }
}
