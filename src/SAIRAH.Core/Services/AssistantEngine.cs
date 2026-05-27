
using SAIRAH.Core.Parsing;
using SAIRAH.Core.Routing;
using SAIRAH.Shared.Commands;
using SAIRAH.Storage.Logging;

namespace SAIRAH.Core.Services
{
    public sealed class AssistantEngine : ICommandExecutor
    {
        private readonly ICommandParser _parser;
        private readonly CommandRouter _router;
        private readonly IAssistantLogger? _logger;

        public AssistantEngine(
            ICommandParser parser,
            CommandRouter router,
            IAssistantLogger? logger = null)
        {
            _parser = parser;
            _router = router;
            _logger = logger;
        }

        public async Task<CommandResult> HandleInputAsync(string input)
        {
            await LogInfoAsync($"Input received: {input}");

            CommandRequest request = _parser.Parse(input);

            await LogInfoAsync($"Parsed intent: {request.Intent}");

            CommandResult result = await _router.ExecuteAsync(request);

            if (result.Success)
            {
                await LogInfoAsync($"Command succeeded: {result.Message}");
            }
            else
            {
                await LogWarningAsync($"Command failed: {result.Message}");
            }

            return result;
        }

        public Task<CommandResult> ExecuteRawAsync(string input)
        {
            return HandleInputAsync(input);
        }

        private Task LogInfoAsync(string message)
        {
            return _logger?.LogInfoAsync(message) ?? Task.CompletedTask;
        }

        private Task LogWarningAsync(string message)
        {
            return _logger?.LogWarningAsync(message) ?? Task.CompletedTask;
        }
    }
}
