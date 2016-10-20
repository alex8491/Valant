using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valant.Data;
using Valant.Models;

namespace Valant.Tests
{
   public  class TestableInventoryStore : InventoryStore
    {
        public TestableInventoryStore() : base()
        {

        }
        public new Dictionary<string, Item> Items
        {
            get { return base.Items; }
        }
        public new Queue< Notification> Notifications
        {
            get { return base.Notifications; }
        }
    }
}
