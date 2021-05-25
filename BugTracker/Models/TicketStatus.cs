using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketStatus //1:1 relationship with a Ticket
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Ticket Status")]
        public string Name { get; set; }
    }
}
