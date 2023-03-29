using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  public class TagSaveDTO
  {
    public int NameId { get; set; } // id from table with tags names
    public int TableId { get; set; } // 1-ingr, 2-dishes, 3-diet
    public int ItemId { get; set; } // item id from table above

  }
}