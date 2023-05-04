using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class ShoppingListItemDTO
  {
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string PortionName { get; set; }
    public bool Checked { get; set; }
  }
}