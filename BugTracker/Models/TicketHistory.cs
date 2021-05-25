using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; } //PK
        [DisplayName("Ticket")]

        public int TicketId { get; set; }  //FK

        public string Property { get; set; }
        [DisplayName("Old Value")]

        public string OldValue { get; set; }
        [DisplayName("New Value")]

        public string NewValue { get; set; }
        [DisplayName("Created Date")]
        [DataType(DataType.DateTime)]

        public DateTime Created { get; set; }
        [DisplayName("Team Member")]

        public string UserId { get; set; }

        public string Description { get; set; }

        //Navigational Properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }



    }
}
