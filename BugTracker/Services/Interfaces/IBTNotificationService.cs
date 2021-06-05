using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
   public interface IBTNotificationService
    {
        Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId);

        Task<List<TicketHistory>> GetProjectTicketsHistories(int projectId);

        Task<List<TicketHistory>> GetCompanyTicketsHistories(int companyId);
    }
}
