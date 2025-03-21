using AituConnectAPI.Bot;
using AituConnectAPI.Keyboards;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AituConnectAPI.Pipelines.Editing.Profile
{
    public class OptionStep : PipelineStep
    {
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public OptionStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService, KeyboardMarkupBuilder keyboardMarkupBuilder) : base(messageSender, pipelineContextService, userService)
        {
            _keyboardMarkup = keyboardMarkupBuilder;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                context.StartedDate = DateTime.UtcNow;

                await _pipelineContextService.UpdateAsync(context);

                List<string> options = new List<string> { "University", "Faculty" };
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                foreach (var option in options)
                {
                    var button = _keyboardMarkup.InitializeInlineKeyboardButton(option, $"{CallbackType.EditProfile.ToString()}:Choosing{option}");
                    buttons.Add(button);
                }

                var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, "Choose the option: ", replyMarkup: keyboard);
            }
            else
            {
                ParseNextStep(context);

                context.Content = string.Empty;

                await _pipelineContextService.UpdateAsync(context);
            }
        }

        private static void ParseNextStep(PipelineContext context)
        {
            if (System.Enum.TryParse(context.Content, out PipelineStepType stepType))
            {
                context.CurrentStep = stepType;
            }
            else
            {
                Console.WriteLine("Invalid step type.");
                return;
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.ChoosingOption;
        }
    }
}
