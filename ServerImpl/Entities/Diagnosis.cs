using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Diagnosis
    {
        [Key, Column(Order = 1)]
        public string TopicId { get; set; }
        [Key, Column(Order = 2)]
        public string SubjectId { get; set; }
        [Key, Column(Order = 3)]
        public int QuestionId { get; set; }
    }
}
