using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class DietDayDTO
  {
    public string Day { get; set; }
    public string Date { get; set; }
    public List<DietDishItemDTO> Dishes { get; set; }

  }
}