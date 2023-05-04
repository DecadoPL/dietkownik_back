using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
  public class ProhibitedIngredient
  {
    public int Id { get; set; }
    public int DietRequirementsId { get; set; }
    public int IngredientId { get; set; }

  }
}