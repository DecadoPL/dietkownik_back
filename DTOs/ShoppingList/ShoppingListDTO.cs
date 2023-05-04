using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class ShoppingListDTO
  {
    public int Id { get; set;}
    public string Name { get; set; }
    public List<ShoppingListItemDTO> Items { get; set; }
  }
}