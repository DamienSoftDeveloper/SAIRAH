
namespace SAIRAH.Storage.Logging
{
    public interface IAssistantLogger
    {
        Task LogInfoAsync(string message);

        Task LogWarningAsync(string message);

        Task LogErrorAsync(string message, Exception? exception = null);
    }
}
