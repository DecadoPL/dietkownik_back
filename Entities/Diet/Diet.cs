using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Diet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DietRequirementsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}