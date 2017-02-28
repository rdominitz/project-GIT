using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class GroupTestAnswer
    {
        public virtual User user { get; set; }
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int TestId { get; set; }
        [Key]
        [Column(Order = 3)]
        public int AnswerId { get; set; }
    }
}
