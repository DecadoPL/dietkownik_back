using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
  public class Tag
  {
    public int Id { get; set; }
    public int NameId { get; set; } // id from table with tags names
    public int TableId { get; set; } // 1-ingr, 2-dishes, 3-diet
    public int ItemId { get; set; } // item id from table above

  }
}