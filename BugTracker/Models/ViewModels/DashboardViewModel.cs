using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Project> Projects { get; set; }

        public List<Ticket> Tickets { get; set; }

        public List<BTUser> Members { get; set; }
        public string Roles { get; set; }

        public int UnassignedTickets { get; set; }
        public int DevelopmentTickets { get; set; }
        public int ResolvedTickets { get; set; }

        public int Developers{ get; set; }

        public int LowPriority { get; set; }
        public int MediumPriority { get; set; }
        public int HighPriority { get; set; }
        public int UrgentPriority { get; set; }
        public int NewDev { get; set; }
        public int Runtime { get; set; }
        public int UIType { get; set; }
        public int Maintenance { get; set; }
        public int Low { get; set; }
        public int Medium { get; set; }
        public int High { get; set; }
        public int Urgent { get; set; }


        public List<Ticket> DevTickets { get; set; }
        public List<Ticket> PMTickets { get; set; }


        public List<Ticket> SubmittedTickets { get; set; }

        public BTUser CurrentUser { get; set; }
    }
}
