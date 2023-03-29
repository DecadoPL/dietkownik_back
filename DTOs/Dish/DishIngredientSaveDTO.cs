using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class DishIngredientSaveDTO
  {
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public int DishId { get; set; }
    public int PortionTypeId { get; set; }
    public string PortionQuantity { get; set; }
  }
}