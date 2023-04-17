using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
  public class DietRequirementsHour
  {
    public int Id { get; set; }
    public int DietRequirementsId { get; set; }
    public string Hour { get; set; }

  }
}