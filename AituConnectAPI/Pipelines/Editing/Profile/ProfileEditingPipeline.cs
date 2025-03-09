using AituConnectAPI.Bot;
using AituConnectAPI.Pipelines.Abstractions;

namespace AituConnectAPI.Pipelines.Editing.Profile
{
    public class ProfileEditingPipeline : Pipeline
    {
        public ProfileEditingPipeline(BotMessageSender messageSender, OptionStep optionStep, UniversityEditingStep universityEditingStep, FacultyEditingStep facultyEditingStep) : base(messageSender)
        {
            _steps.Add(optionStep);
            _steps.Add(universityEditingStep);
            _steps.Add(facultyEditingStep);
        }
    }
}
