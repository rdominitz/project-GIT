using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Group
    {
        [Key, Column(Order = 1)]
        public string AdminId { get; set; }
        [Key, Column(Order = 2)]
        public string name { get; set; }
    }
}
