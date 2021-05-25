using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Member Comment")]
        public string Comment { get; set; }
        [DisplayName("Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }
        [DisplayName("Ticket")]

        public int TicketId { get; set; } //FK
        [DisplayName("Team Member")]

        public string UserId { get; set; }
        //Navigational Properties
        public virtual ICollection<TicketComment> Ticket { get; set; } = new HashSet<TicketComment>();

        public virtual BTUser User { get; set; }


    }
}
