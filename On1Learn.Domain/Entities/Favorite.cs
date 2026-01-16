using On1Learn.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public string? Note { get; set;  }  
    }
}
