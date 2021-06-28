using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIN_Client
{
    class ChatMessage
    {
        public ChatMessage(DateTime time, string content)
        {
            this.time = time;
            this.content = content;
        }

        public DateTime time { get; set; }
        public string content { get; set; }
    }
}
