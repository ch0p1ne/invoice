using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class GreetingMessage
    {
        public int GreetingMessageId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? EventName { get; set; }
        public DateOnly BeginDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
