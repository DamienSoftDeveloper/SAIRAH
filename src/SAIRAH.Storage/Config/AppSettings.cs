

namespace SAIRAH.Storage.Config
{
    public sealed class AppSettings
    {
        public string AssistantName { get; set; } = "SAIRAH";

        public bool EnableLogging { get; set; } = true;

        public string LogFolderName { get; set; } = "logs";

        public bool StartInOnlineMode { get; set; } = true;

        public bool EnableTextToSpeech { get; set; } = false;

        public bool EnableVoiceInput { get; set; } = false;
    }
}

