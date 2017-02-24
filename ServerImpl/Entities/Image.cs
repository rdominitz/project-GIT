using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Image
    {
        [Key]
        [Column(Order = 1)]
        public string ImageId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question question { get; set; }
    }
}
