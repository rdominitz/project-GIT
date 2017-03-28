using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        public string SubjectId { get; set; }
        [Required]
        public bool normal { get; set; }
        public string text { get; set; }
        [Required]
        public DateTime timeAdded { get; set; }
        [Required]
        public int level { get; set; }
        [Required]
        public double points { get; set; }
    }
}
