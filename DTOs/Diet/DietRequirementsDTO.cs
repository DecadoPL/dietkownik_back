using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class DietRequirementsDTO
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public MacroDTO Macro { get; set; }
    public MicroDTO Micro { get; set; }
    public List<RequiredTagDTO> RequiredTags { get; set; }
    public List<ProhibitedTagDTO> ProhibitedTags { get; set; }
    public List<ProhibitedIngredientDTO> ProhibitedIngredients { get; set; }
    public List<RequiredIngredientDTO> RequiredIngredients { get; set; }

  }
}