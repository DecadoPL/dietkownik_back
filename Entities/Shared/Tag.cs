using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
  public class Tag
  {
    public int Id { get; set; }
    public int NameId { get; set; }
    public int TableId { get; set; }
    public int ItemId { get; set; }

  }
}