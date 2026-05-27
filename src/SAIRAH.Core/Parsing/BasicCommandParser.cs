using SAIRAH.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Core.Parsing
{
    public sealed class BasicCommandParser : ICommandParser
    {
        public CommandRequest Parse(string input)
        {
            string cleanedInput = input.Trim();

            if (string.IsNullOrWhiteSpace(cleanedInput))
            {
                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "empty"
                };
            }

            string lowerInput = cleanedInput.ToLowerInvariant();

            if (lowerInput.StartsWith("run workflow "))
            {
                string workflowName = cleanedInput["run workflow ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "run_workflow",
                    Parameters = new Dictionary<string, string>
                    {
                        ["name"] = workflowName
                    }
                };
            }

            if (lowerInput.StartsWith("start workflow "))
            {
                string workflowName = cleanedInput["start workflow ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "run_workflow",
                    Parameters = new Dictionary<string, string>
                    {
                        ["name"] = workflowName
                    }
                };
            }

            if (lowerInput.StartsWith("start "))
            {
                string workflowName = cleanedInput["start ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "run_workflow",
                    Parameters = new Dictionary<string, string>
                    {
                        ["name"] = workflowName
                    }
                };
            }

            if (lowerInput.StartsWith("open folder "))
            {
                string folder = cleanedInput["open folder ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "open_folder",
                    Parameters = new Dictionary<string, string>
                    {
                        ["folder"] = folder
                    }
                };
            }

            if (lowerInput.StartsWith("open website "))
            {
                string url = cleanedInput["open website ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "open_website",
                    Parameters = new Dictionary<string, string>
                    {
                        ["url"] = url
                    }
                };
            }

            if (lowerInput.StartsWith("open site "))
            {
                string url = cleanedInput["open site ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "open_website",
                    Parameters = new Dictionary<string, string>
                    {
                        ["url"] = url
                    }
                };
            }

            if (lowerInput.StartsWith("create folder "))
            {
                return ParseCreateFolder(cleanedInput, input);
            }

            if (lowerInput == "clear chat")
            {
                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "clear_chat"
                };
            }

            if (lowerInput.StartsWith("open "))
            {
                string target = cleanedInput["open ".Length..].Trim();

                return new CommandRequest
                {
                    RawInput = input,
                    Intent = "open_app",
                    Parameters = new Dictionary<string, string>
                    {
                        ["app"] = target
                    }
                };
            }

            return new CommandRequest
            {
                RawInput = input,
                Intent = "unknown"
            };
        }

        private static CommandRequest ParseCreateFolder(string cleanedInput, string rawInput)
        {
            string commandBody = cleanedInput["create folder ".Length..].Trim();

            string folderName = commandBody;
            string location = "desktop";

            const string onKeyword = " on ";

            int onIndex = commandBody.LastIndexOf(onKeyword, StringComparison.OrdinalIgnoreCase);

            if (onIndex >= 0)
            {
                folderName = commandBody[..onIndex].Trim();
                location = commandBody[(onIndex + onKeyword.Length)..].Trim();
            }

            return new CommandRequest
            {
                RawInput = rawInput,
                Intent = "create_folder",
                Parameters = new Dictionary<string, string>
                {
                    ["name"] = folderName,
                    ["location"] = location
                }
            };
        }
    }
}
