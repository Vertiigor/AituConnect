using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class RegistrationPipeline
    {
        private readonly List<PipelineStep> _steps;
        protected readonly BotMessageSender _messageSender;

        public RegistrationPipeline(BotMessageSender messageSender, UniversityStep universityStep, FacultyStep facultyStep, CongratulationStep congratulationStep)
        {
            _messageSender = messageSender;
            _steps = new List<PipelineStep>()
            {
                universityStep,
                facultyStep,
                congratulationStep
            };
        }

        public async Task ExecuteAsync(PipelineContext context)
        {
            if (!context.IsCompleted)
            {
                foreach (var step in _steps)
                {
                    if (step.IsApplicable(context))
                    {
                        await step.ExecuteAsync(context);
                        break; // Execute only the current step
                    }
                }
            }
        }
    }
}
