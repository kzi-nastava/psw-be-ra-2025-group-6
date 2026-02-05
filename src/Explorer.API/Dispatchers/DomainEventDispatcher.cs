using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading.Tasks;

namespace Explorer.API.Dispatchers
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IServiceProvider provider, ILogger<DomainEventDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            _logger.LogInformation("Dispatching event of type {EventType}", domainEvent.GetType().Name);

            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _provider.GetServices(handlerType);

            if (!handlers.Any())
            {
                _logger.LogWarning("No handlers found for event {EventType}", domainEvent.GetType().Name);
            }

            foreach (var handlerObj in handlers) // don't use dynamic here
            {
                var handler = handlerObj as dynamic; // only this cast is dynamic
                await handler.Handle((dynamic)domainEvent);
            }


        }
    }
}
