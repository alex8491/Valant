using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valant.Models
{
    public class Item
    {
       //Unecessary for in-memory data store.
       //[Key]
       // public int Id { get; set; }
        public DateTime Expiration { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
    }
}