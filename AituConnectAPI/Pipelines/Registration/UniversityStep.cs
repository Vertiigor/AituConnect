using AituConnectAPI.Bot;
using AituConnectAPI.Keyboards;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AituConnectAPI.Pipelines.Registration
{
    public class UniversityStep : PipelineStep
    {
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public UniversityStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService, KeyboardMarkupBuilder keyboardMarkup) : base(messageSender, pipelineContextService, userService)
        {
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
                    var button = _keyboardMarkup.InitializeInlineKeyboardButton(university, $"{CallbackType.ChooseUniversity.ToString()}:{university}");
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
                context.CurrentStep = PipelineStepType.Faculty;    // Move to the next step
                context.Content = string.Empty;
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.Univeristy;
        }
    }
}
