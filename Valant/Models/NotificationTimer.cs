using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using Valant.Models;

namespace Valant.Data
{
    // Extends Timer to store an association with an inventory item.
    public class NotificationTimer : Timer
    {
        public Item AssociatedItem { get; set; }

        public NotificationTimer(Item asociatedItem)
        {
            AssociatedItem = asociatedItem;
        }
    }
}