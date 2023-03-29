using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class DishReadDTO
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Image { get; set; }
    public string Portions { get; set; }
    public string Description { get; set; }
    public string Recipe { get; set; }
    public string CookingTime { get; set; }
    public List<DishIngredientReadDTO> Ingredients { get; set; }
    public MacroDTO Macro { get; set; }
    public MicroDTO Micro { get; set; }
    public List<TagReadDTO> Tags { get; set; }

  }
}