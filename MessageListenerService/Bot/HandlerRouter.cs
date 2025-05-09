using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.Bot
{
    public class HandlerRouter
    {
        private readonly Dictionary<(string Pipeline, string Step), StepHandler> _handlers;

        public HandlerRouter(IEnumerable<StepHandler> handlers)
        {
            _handlers = new Dictionary<(string Pipeline, string Step), StepHandler>();

            // Register all handlers in the dictionary
            foreach (var handler in handlers)
            {
                _handlers[(handler.PipelineName, handler.StepName)] = handler;
            }
        }

        public bool TryGetValue((string Pipeline, string Step) key, out StepHandler handler)
        {
            return _handlers.TryGetValue(key, out handler);
        }
    }
}
