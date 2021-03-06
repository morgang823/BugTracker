using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    //service for notification
    public class BTNotificationsService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTCompanyInfoService  _infoservice;

        public BTNotificationsService(ApplicationDbContext context, IEmailSender emailSender, IBTCompanyInfoService infoservice)
        {
            _context = context;
            _emailSender = emailSender;
            _infoservice = infoservice;
        }

        public async  Task AdminsNotificationAsync(Notification notification, int companyId)
        {
            try
            {
                //Get company admin
                List<BTUser> admins = await _infoservice.GetMembersInRoleAsync("Admin", companyId);
                foreach(BTUser bTUser in admins)
                {
                    notification.RecipientId = bTUser.Id;

                        await EmailNotificationAsync(notification, notification.Title);
                }

            }
            catch
            {
                throw;
            }
        }

        public async Task EmailNotificationAsync(Notification notification, string emailSubject)
        {
            try
            {
                BTUser btUser = await _context.Users.FindAsync(notification.RecipientId);
                //Send Email
                string btUserEmail = btUser.Email;
                string message = notification.Message;
                await _emailSender.SendEmailAsync(btUser.Email, emailSubject, message);

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications =await _context.Notification
                            .Include(n => n.Recipient)
                            .Include(n => n.Sender)
                            .Include(n => n.Ticket)
                            .ThenInclude(t => t.Project)
                            .Where(n => n.RecipientId == userId).ToListAsync();

                return notifications;
                                            
            }
            catch
            {
                throw;
            }
        }

        public async  Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notification
                            .Include(n => n.Recipient)
                            .Include(n => n.Sender)
                            .Include(n => n.Ticket)
                            .ThenInclude(t => t.Project)
                            .Where(n => n.SenderId == userId).ToListAsync();

                return notifications;

            }
            catch
            {
                throw;
            }
        }

        public async Task MembersNotificationAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach(BTUser btUser in members)
                {

                    notification.RecipientId = btUser.Id;

                    await EmailNotificationAsync(notification, notification.Title);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public Task SMSNotificationAsync(string phone, Notification notification)
        {
            throw new NotImplementedException();
        }
    }
}
