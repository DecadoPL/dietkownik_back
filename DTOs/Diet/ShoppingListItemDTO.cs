using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class ShoppingListItemDTO
  {
    public string IngredientName { get; set; }
    public string Quantity { get; set; }
    public string PortionType { get; set; }
  }
}