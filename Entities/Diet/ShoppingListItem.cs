using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class ShoppingListItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string PortionType { get; set; }
        public bool Checked { get; set; }

    }
}