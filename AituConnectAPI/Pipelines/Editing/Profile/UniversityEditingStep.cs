using AituConnectAPI.Bot;
using AituConnectAPI.Keyboards;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AituConnectAPI.Pipelines.Editing.Profile
{
    public class UniversityEditingStep : PipelineStep
    {
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public UniversityEditingStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService, KeyboardMarkupBuilder keyboardMarkupBuilder) : base(messageSender, pipelineContextService, userService)
        {
            _keyboardMarkup = keyboardMarkupBuilder;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                List<string> universities = new List<string> { "AITU", "MIT", "Harvard", "UCLA", "University of Utah" };
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                foreach (var university in universities)
                {
                    var button = _keyboardMarkup.InitializeInlineKeyboardButton(university, $"{CallbackType.ChooseUniversity.ToString()}:{university}");
                    buttons.Add(button);
                }

                var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, $"Current: {user.University}\n(Editing) Enter the name of university you're styding in: ", replyMarkup: keyboard);
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.University = context.Content;
                await _userService.UpdateAsync(user);

                context.IsCompleted = true;
                context.FinishedDate = DateTime.UtcNow;
                await _pipelineContextService.DeleteAsync(context.Id);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.Univeristy;
        }
    }
}
