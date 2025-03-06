using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AituConnectAPI.Pipelines.Registration
{
    public class UniversityStep : PipelineStep
    {
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IUserService _userService;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public UniversityStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService, KeyboardMarkupBuilder keyboardMarkup) : base(messageSender)
        {
            _pipelineContextService = pipelineContextService;
            _userService = userService;
            _keyboardMarkup = keyboardMarkup;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                List<string> universities = new List<string> { "AITU", "MIT", "Harvard", "UCLA", "University of Utah" };
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                foreach (var university in universities)
                {
                    var button = _keyboardMarkup.InitializeInlineKeyboardButton(university, $"choose_university:{university}");
                    buttons.Add(button);
                }

                var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, "Enter the name of university you're styding in: ", replyMarkup: keyboard);
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.University = context.Content;
                await _userService.UpdateAsync(user);

                context.StartedDate = DateTime.UtcNow;
                context.CurrentStep = "FACULTY";    // Move to the next step
                context.Content = "";
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "UNIVERSITY";
        }
    }
}
