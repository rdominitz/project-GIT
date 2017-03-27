using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class GroupMember
    {
        [Key, Column(Order = 1)]
        public string GroupName { get; set; }
        [Key, Column(Order = 2)]
        public string AdminId { get; set; }
        [Key, Column(Order = 3)]
        public string UserId { get; set; }
        [Required]
        public bool invitationAccepted { get; set; }
    }
}
