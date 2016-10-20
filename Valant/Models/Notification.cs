using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valant.Models
{
    public class Notification
    {
        public NotificationType Type { get; set; }
        public Item AffectedItem { get; set; }
        public DateTime Timestamp { get; set; }
        
    }
}