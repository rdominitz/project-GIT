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
        [Key]
        public int GroupId { get; set; }
        [Required]
        public string AdminId { get; set; }
        [Required]
        public string name { get; set; }
    }
}
