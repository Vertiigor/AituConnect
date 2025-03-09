using AituConnectAPI.Bot;
using AituConnectAPI.Pipelines.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class RegistrationPipeline : Pipeline
    {
        public RegistrationPipeline(BotMessageSender messageSender, UniversityStep universityStep, FacultyStep facultyStep, CongratulationStep congratulationStep) : base(messageSender)
        {
            _steps.Add(universityStep);
            _steps.Add(facultyStep);
            _steps.Add(congratulationStep);
        }
    }
}
