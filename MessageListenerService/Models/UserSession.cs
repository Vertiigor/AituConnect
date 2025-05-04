namespace MessageListenerService.Models
{
    public class UserSession
    {
        public string ChatId { get; set; }
        public string CurrentPipeline { get; set; } // e.g., "Registration"
        public string CurrentStep { get; set; }     // e.g., "ChooseUniversity"
        public Dictionary<string, string> Data { get; set; } = new();
    }

}
