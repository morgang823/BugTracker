using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketPriority //1:1 relationship with a Ticket
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Ticket Priority")]
        public string Name { get; set; }
    }
}
