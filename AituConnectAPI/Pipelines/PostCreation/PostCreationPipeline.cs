using AituConnectAPI.Bot;
using AituConnectAPI.Pipelines.Abstractions;

namespace AituConnectAPI.Pipelines.PostCreation
{
    public class PostCreationPipeline : Pipeline
    {
        public PostCreationPipeline(BotMessageSender messageSender, TitleStep titleStep, ContentStep contentStep) : base(messageSender)
        {
            _steps.Add(titleStep);
            _steps.Add(contentStep);
        }
    }
}
