using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAIRAH.App.Bootstrap;
using SAIRAH.App.Models;
using SAIRAH.Core.Services;
using SAIRAH.Storage.Workflows;

namespace SAIRAH.App.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly AssistantEngine _assistantEngine;

        [ObservableProperty]
        private string _commandInput = string.Empty;

        [ObservableProperty]
        private string _assistantStatus = "Offline Local Mode";

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public MainWindowViewModel()
        {
            _assistantEngine = AppBootstrapper.CreateAssistantEngine();

            Messages.Add(new ChatMessage
            {
                Sender = "SAIRAH",
                Message = "SAIRAH Core is active. Type a command to begin."
            });
        }

        [RelayCommand]
        private async Task SendCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(CommandInput))
            {
                return;
            }

            string userInput = CommandInput.Trim();

            Messages.Add(new ChatMessage
            {
                Sender = "You",
                Message = userInput
            });

            CommandInput = string.Empty;

            if (string.Equals(userInput, "clear chat", StringComparison.OrdinalIgnoreCase))
            {
                Messages.Clear();

                Messages.Add(new ChatMessage
                {
                    Sender = "SAIRAH",
                    Message = "Chat cleared."
                });

                return;
            }

            var result = await _assistantEngine.HandleInputAsync(userInput);

            Messages.Add(new ChatMessage
            {
                Sender = "SAIRAH",
                Message = result.Message
            });
        }

        public async Task InitialiseAsync()
        {
            try
            {
                IWorkflowRepository workflowRepository = new JsonWorkflowRepository();
                DefaultWorkflowSeeder workflowSeeder = new DefaultWorkflowSeeder(workflowRepository);

                await workflowSeeder.SeedAsync();

                Messages.Add(new ChatMessage
                {
                    Sender = "SAIRAH",
                    Message = "Default workflows are ready."
                });
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "SAIRAH",
                    Message = $"Workflow setup failed: {ex.Message}"
                });
            }
        }

    }
}
