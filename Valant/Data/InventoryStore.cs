using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using Valant.Models;

namespace Valant.Data
{
    public class InventoryStore
    {
        protected Dictionary<string, Item> Items { get; set; }
        private Dictionary<string, Timer> ExpirationTimers { get; set; }
        protected Queue<Notification> Notifications { get; set; }
        private const int NOTIFICATION_RECORD_LENGTH = 100000;
        public InventoryStore()
        {
            Items = new Dictionary<string, Item>();
            ExpirationTimers = new Dictionary<string, Timer>();
            Notifications = new Queue<Notification>(NOTIFICATION_RECORD_LENGTH);
        }
        public bool AddItem(Item item)
        {
            // If this label is already associated with the item.
            if (Items.ContainsKey(item.Label))
            {
                return false;
            }
            // Add the item to the inventory and create a timer for its expiration.
            Items.Add(item.Label, item);
            SetExpirationTimer(item);
            return true;
        }

        public Item RemoveItem(string label)
        {
            // If the item doesn't exist
            if (!Items.ContainsKey(label))
            {
                return null;
            }
            var item = Items[label];
            // Otherwise remove the item and shut its expiration timer off.
            RemoveExpirationTimer(item.Label);
            Items.Remove(item.Label);

            // Add a notification the item was removed.
            var notification = new Notification
            {
                AffectedItem = item,
                Type = NotificationType.ItemRemoved,
                Timestamp = DateTime.Now
            };
            Notifications.Enqueue(notification);
            return item;
        }

        private void SetExpirationTimer(Item item)
        {
            // Clear any existing timer.
            RemoveExpirationTimer(item.Label);
            // Calculate the timer duration.
            var now = DateTime.Now;
            var expirationInterval = item.Expiration - now;

            // If it's already expired
            if (item.Expiration <= now)
            {
                var notification = new Notification
                {
                    AffectedItem = item,
                    Timestamp = DateTime.Now,
                    Type = NotificationType.ItemExpired
                };
                Notifications.Enqueue(notification);
            }
            //Start the expiration timer otherwise
            else
            {
                var newTimer = new NotificationTimer(item)
                {
                    Interval = expirationInterval.TotalMilliseconds, // Interval is in miliseconds.
                    AutoReset = false,
                };

                // Add our timer event to our timer, and add our timer to the timer collection.
                newTimer.Elapsed += CreateTimerNotification;
                ExpirationTimers.Add(item.Label, newTimer);

                // Start the timer.
                newTimer.Start();
            }
        }

        private void CreateTimerNotification(object sender, EventArgs e)
        {
            var timer = (NotificationTimer)sender;
            // Create a notification that the timer's associated item expired.
            var notification = new Notification
            {
                AffectedItem = timer.AssociatedItem,
                Timestamp = DateTime.Now,
                Type = NotificationType.ItemExpired
            };
            Notifications.Enqueue(notification);
        }

        private void RemoveExpirationTimer(string label)
        {
            if (ExpirationTimers.ContainsKey(label))
            {
                var timer = ExpirationTimers[label];
                timer.Elapsed -= CreateTimerNotification;
                timer.Close();
                ExpirationTimers.Remove(label);
            }
        }

        //Clean up timers in the finalizer to prevent potential memory leaks.
        ~InventoryStore()
        {
            foreach (var timer in ExpirationTimers.Values)
            {
                timer.Elapsed -= CreateTimerNotification;
                timer.Close();
            }
        }

    }
}