using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime timeAdded { get; set; }
        [Required]
        public int questionLevel { get; set; }
        [Required]
        public bool isCorrectAnswer { get; set; }
        [Required]
        public bool normal { get; set; }
        [Required]
        public int normalityCertainty { get; set; }
    }
}
