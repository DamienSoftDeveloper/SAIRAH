using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.App.Models
{
    public sealed class ChatMessage
    {
        public string Sender { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public DateTime Timestamp { get; init; } = DateTime.Now;
    }
}
