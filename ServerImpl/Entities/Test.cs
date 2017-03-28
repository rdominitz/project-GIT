using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Test
    {
        [Key]
        public int TestId { get; set; }
        [Required]
        public string testName { get; set; }
        [Required]
        public string AdminId { get; set; }
    }
}
