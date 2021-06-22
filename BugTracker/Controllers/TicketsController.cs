using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using BugTracker.Extensions;
using Microsoft.AspNetCore.Identity;
using BugTracker.Models.ViewModels;
using BugTracker.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTCompanyInfoService _infoService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTHistoryService _historyService;
        private readonly IBTNotificationService _notificationService;
        private readonly IEmailSender _emailService;
        public TicketsController(ApplicationDbContext context, IBTCompanyInfoService infoService, IBTProjectService projectService, UserManager<BTUser> userManager, IBTTicketService ticketService, IBTHistoryService historyService, IBTNotificationService notificationService, IEmailSender emailService)
        {
            _context = context;
            _infoService = infoService;
            _projectService = projectService;
            _userManager = userManager;
            _ticketService = ticketService;
            _historyService = historyService;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        // GET: All Tickets regardless of company
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ticket.Include(t => t.DeveloperUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());
        }

        //Get all tickets for a company
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllTickets()
        {
            //Get CompanyID
            int companyId = User.Identity.GetCompanyId().Value;
            var tickets = (await _infoService.GetAllTicketsAsync(companyId));
            return View(tickets);
        }
        //Get
        [HttpGet]
        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> AssignTicket(int? ticketId)
        {
            if(!ticketId.HasValue)
            {
                return NotFound();
            }
            AssignDeveloperViewModel model = new();

            //Get CompanyID
            int companyId = User.Identity.GetCompanyId().Value;
            model.Ticket = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);
            model.Developers = new SelectList(await _projectService.DevelopersOnProjectAsync(model.Ticket.ProjectId),"Id","FullName");
            return View(model);
        }
        //POST
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> AssignTicket(AssignDeveloperViewModel viewModel)
        {
            if(!string.IsNullOrEmpty(viewModel.DeveloperId))
            {
                int companyId = User.Identity.GetCompanyId().Value;

                BTUser user = await _userManager.GetUserAsync(User);
                BTUser projectManager = await _projectService.GetProjectManagerAsync(viewModel.Ticket.ProjectId);

                Ticket oldTicket = await _context.Ticket
                                    .Include(t => t.TicketPriority)
                                    .Include(t => t.TicketStatus)
                                    .Include(t => t.TicketType)
                                    .Include(t => t.Project)
                                    .Include(t => t.DeveloperUser)
                                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                await _ticketService.AssignTicketAsync(viewModel.Ticket.Id, viewModel.DeveloperId);

                Ticket newTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, user.Id);
                Notification notification = new()
                {
                    TicketId = newTicket.Id,
                    Title = "You've Been Assigned A Ticket",
                    Message = $"New Ticket: {newTicket?.Title} was Assigned To You By {user?.FullName}",
                    Created = DateTimeOffset.Now,
                    SenderId = user?.Id,
                    RecipientId = viewModel.DeveloperId,
                };


                if (viewModel.DeveloperId != null)
                {
                    await _notificationService.SaveNotificationAsync(notification);
                    await _notificationService.EmailNotificationAsync(notification, "message has been sent.");
                }

            }
            return RedirectToAction("Details", new { id = viewModel.Ticket.Id });
        }
        [Authorize]
        public async Task<IActionResult> MyTickets()
        {
            //Get UserID
            var userId = _userManager.GetUserId(User);
            var devtickets = await _ticketService.GetAllTicketsByRoleAsync("Developer", userId);
            var subtickets = await _ticketService.GetAllTicketsByRoleAsync("Submitter", userId);
            var viewModel = new MyTicketsViewModel()
            {
                DevTickets = devtickets,
                SubmittedTickets = subtickets
            };

            return View(viewModel);
        }
        [Authorize]

        public IActionResult ShowFile(int id)
        {
            TicketAttachment ticketAttachment = _context.TicketAttachment.Find(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        // GET: Tickets/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .Include(t => t.History).ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager, Developer, Submitter")]

        public async Task<IActionResult> Create()
        {
            //get current user
            BTUser btUser =await _userManager.GetUserAsync(User);

            //get current user's company id
            int companyId = User.Identity.GetCompanyId().Value;
            if(User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");

            }
            else
            {
            ViewData["ProjectId"] = new SelectList(await _projectService.ListUserProjectsAsync(btUser.Id), "Id", "Name");

            }

            //TODO: FIlter List
            
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name");

            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager, Developer, Submitter")]

        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                ticket.Created = DateTimeOffset.Now;
                string userId = _userManager.GetUserId(User);
                ticket.OwnerUserId = userId;
                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync("New")).Value;
                _context.Add(ticket);
                await _context.SaveChangesAsync();

                #region Add History
                //AddHistory
                Ticket newTicket = await _context.Ticket
                                    .Include(t => t.TicketPriority)
                                    .Include(t => t.TicketStatus)
                                    .Include(t => t.TicketType)
                                    .Include(t => t.Project)
                                    .Include(t => t.DeveloperUser)
                                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);
                await _historyService.AddHistoryAsync(null, newTicket, btUser.Id);
                #endregion

                #region Notification

                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity.GetCompanyId().Value;
                Notification notification = new()
                {
                    TicketId = ticket.Id,
                        Title = "New Ticket",
                        Message = $"New Ticket{ticket?.Title} was created by {btUser?.FullName}",
                        Created = DateTimeOffset.Now,
                        SenderId = btUser?.Id,
                        RecipientId = projectManager?.Id,
                    };

                if (projectManager!=null)
                {
                    await _notificationService.SaveNotificationAsync(notification);
                    await _notificationService.EmailNotificationAsync(notification, "message has been sent.");
                }
                else
                {
                    await _notificationService.AdminsNotificationAsync(notification, companyId);
                }
                
                #endregion
                {

                    return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
                }
            }
                ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
                ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
        }
        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager, Developer, Submitter")]

        public async Task<IActionResult> Edit(int? id)
        {
            //get current user
            BTUser user = await _userManager.GetUserAsync(User);
            var role = await _userManager.GetRolesAsync(user);

            
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager, Developer, Submitter")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ArchiveDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser btUser = await _userManager.GetUserAsync(User);
            BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

                Ticket oldTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);
                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }


                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //Add History
                Ticket newTicket = await _context.Ticket
    .Include(t => t.TicketPriority)
    .Include(t => t.TicketStatus)
    .Include(t => t.TicketType)
    .Include(t => t.Project)
    .Include(t => t.DeveloperUser)
    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);
                if (ticket.DeveloperUserId != null)
                {
                    Notification notification = new()
                    {
                        TicketId = ticket.Id,
                        Title = "A ticket assigned to you has been modified",
                        Message = $"New Ticket{ticket?.Title} was edited by {btUser?.FullName}",
                        Created = DateTimeOffset.Now,
                        SenderId = btUser?.Id,
                        RecipientId = ticket.DeveloperUserId
                    };

                    await _notificationService.SaveNotificationAsync(notification);
                    await _notificationService.EmailNotificationAsync(notification, "message has been sent.");


                }
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);
            //return View(ticket);
            return RedirectToAction("Details", "Ticket", new {ticket.Id});

        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        [Authorize(Roles = "ProjectManager")]

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
