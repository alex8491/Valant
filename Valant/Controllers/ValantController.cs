using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Valant.Data;
using Valant.Models;

namespace Valant.Controllers
{
    // Routing:  api/Valant/{action}/{id}
    /// <summary>
    /// 
    /// </summary>
    public class ValantController : ApiController
    {
        private InventoryStore ItemRepository { get; set; }

        public ValantController()
        {
            ItemRepository = new InventoryStore();
        }
        /// <summary>
        /// Removes an item with the given label from the inventory. No behavioral difference between Get/Post/Delete
        /// </summary>
        /// <param name="label">The unique label of the item to remove.</param>
        /// <returns>The item removed.</returns>
        [HttpGet]
        [HttpPost]
        [HttpDelete]
        public Item Remove(string label)
        {
            return ItemRepository.RemoveItem(label);
        }

        /// <summary>
        /// Adds an item to the inventory. The item's label and expiration date must be set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was successfuly added, false otherwise.</returns>
        [HttpPost]
        public bool Add([FromBody]Item item)
        {
          return  ItemRepository.AddItem(item);
        }
    }
}
