using MessageProducerService.StepHandlers.Abstractions;

namespace MessageProducerService.Services
{
    public class HandlerRouter
    {
        private readonly Dictionary<string, StepHandler> _handlers;

        public HandlerRouter(IEnumerable<StepHandler> handlers)
        {
            _handlers = new Dictionary<string, StepHandler>();

            // Register all handlers in the dictionary
            foreach (var handler in handlers)
            {
                _handlers[handler.StepName] = handler;
            }
        }

        public bool TryGetValue(string key, out StepHandler handler)
        {
            return _handlers.TryGetValue(key, out handler);
        }
    }
}
