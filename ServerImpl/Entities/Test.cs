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
        [Required]
        public string subject { get; set; }

        public override string ToString()
        {
            return "ID: " + TestId + ", Creator email: " + AdminId + ", Name: " + testName;
        }
    }
}
