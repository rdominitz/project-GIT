using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserLevel
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public string TopicId { get; set; }
        [Key, Column(Order = 3)]
        public string SubjectId { get; set; }
        [Required]
        public int level { get; set; }
        [Required]
        public int timesAnswered { get; set; }
        [Required]
        public int timesAnsweredCorrectly { get; set; }
    }
}
