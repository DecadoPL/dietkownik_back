using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
  public class IngredientPortion
  {
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public int PortionNameId { get; set; }
    public string Quantity { get; set; }

  }
}