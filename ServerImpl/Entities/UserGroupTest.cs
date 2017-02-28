using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserGroupTest
    {
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public int GroupId { get; set; }
        [Key, Column(Order = 3)]
        public int testId { get; set; }
        [Required]
        public bool completed { get; set; }
    }
}
