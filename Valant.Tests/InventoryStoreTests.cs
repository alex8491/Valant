using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Valant.Models;
using System.Linq;
namespace Valant.Tests
{
    // Unit tests are written according to arrange/act/assert principle.
    // Some arranging is done in TestInitialize.
    [TestClass]
    public class InventoryStoreTests
    {
        private TestableInventoryStore Inventory { get; set; }
        private Item DummyItem { get; set; }

        [TestInitialize]
        public void Init()
        {
            Inventory = new TestableInventoryStore();
            // Expires in a week.
            DummyItem = new Item { Label = "foo", Type = "bar", Expiration = DateTime.Now + new TimeSpan(7,0,0,0)};
        }

        [TestMethod]
        public void WhenItemIsAddedDataRepositoryIndexesItemByLabelAndReturnsTrue()
        {
          var success =  Inventory.AddItem(DummyItem);

            Assert.AreEqual(success, true);
            Assert.AreEqual(DummyItem, Inventory.Items[DummyItem.Label]);
        }

        [TestMethod]
        public void WhenDuplicateItemLabelIsAddedDataRepositoryIgnoresAndReturnsFalse()
        {
            var newItem = new Item { Label = DummyItem.Label, Expiration = DateTime.Now };

            Inventory.AddItem(DummyItem);
            var newItemStatus = Inventory.AddItem(newItem);

            Assert.AreEqual(DummyItem, Inventory.Items[DummyItem.Label]);
            Assert.AreEqual(false, newItemStatus);
        }



        [TestMethod]
        public void WhenItemIsRemovedDataRepositoryRemovesItemI()
        {
            Inventory.Items.Add(DummyItem.Label, DummyItem);

            Inventory.RemoveItem(DummyItem.Label);

            Assert.AreEqual(0, Inventory.Items.Count);
        }


        [TestMethod]
        public void WhenItemIsRemovedNotificationIsGenerated()
        {
            Inventory.Items.Add(DummyItem.Label, DummyItem);

            Inventory.RemoveItem(DummyItem.Label);
            // Note that DummyItem expires far in the future.
            Assert.AreEqual(1, Inventory.Notifications.Count);
        }
        
        public void WhenItemRemovedThatDoesNotExistNullIsReturned()
        {
            Inventory.AddItem(DummyItem);

            var item = Inventory.RemoveItem("foobar");

            Assert.AreEqual(null, item);
        }
        [TestMethod]
        public void WhenItemExpiresNotificationIsGenerated()
        {
            // Note: Must set the expiration date before adding to inventory.
            DummyItem.Expiration = DateTime.Now + new TimeSpan(0,0,3);
            Inventory.AddItem(DummyItem);

            // Wait for the item to expire.
            System.Threading.Thread.Sleep(3500);

            // Query in case multiple notifications would be generated in the future.
            var expirationNotificationPresent = Inventory.Notifications.Any(n => n.Type == NotificationType.ItemExpired);
            Assert.IsTrue(expirationNotificationPresent);
        }

        [TestMethod]
        public void WhenExpiredItemIsAddedNotificationIsGenerated()
        {
            //Expired yesterday.
            DummyItem.Expiration = DateTime.Now - new TimeSpan(1, 2, 3, 4);

            Inventory.AddItem(DummyItem);
            // Query in case multiple notifications would be generated in the future.
            var expirationNotificationPresent = Inventory.Notifications.Any(n => n.Type == NotificationType.ItemExpired);
            Assert.IsTrue(expirationNotificationPresent);
        }


    }
}
