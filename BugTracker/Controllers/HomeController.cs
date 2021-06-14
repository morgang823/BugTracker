using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _infoService;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTRolesService _roleService;

        public HomeController(ILogger<HomeController> logger, UserManager<BTUser> userManager, IBTCompanyInfoService infoService, IBTProjectService projectService, IBTTicketService ticketService, IBTRolesService roleService)
        {
            _logger = logger;
            _userManager = userManager;
            _infoService = infoService;
            _projectService = projectService;
            _ticketService = ticketService;
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Landing()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            BTUser user = await _userManager.GetUserAsync(User);
            DashboardViewModel viewModel = new()
            {
                Projects = await _projectService.GetAllProjectsByCompanyAsync(companyId),
                Tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId),
                DevTickets = await _ticketService.GetAllTicketsByRoleAsync("Developer", user.Id),
                SubmittedTickets = await _ticketService.GetAllTicketsByRoleAsync("Submitter", user.Id),
                Members = await _infoService.GetAllMembersAsync(companyId),
                Developers = (await _infoService.GetMembersInRoleAsync("Developer", companyId)).Count,
                CurrentUser = user,
                //Roles = await _roleService.ListUserRolesAsync(user),
                LowPriority = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, "Low")).Count,
                MediumPriority = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, "Medium")).Count,
                HighPriority = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, "High")).Count,
                UrgentPriority = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, "Urgent")).Count,
                UnassignedTickets = (await _ticketService.GetAllTicketsByStatusAsync(companyId, "Unassigned")).Count,
                DevelopmentTickets = (await _ticketService.GetAllTicketsByStatusAsync(companyId, "Development")).Count,
                //Type
                NewDev = (await _ticketService.GetAllTicketsByTypeAsync(companyId, "New Development")).Count,
                Runtime = (await _ticketService.GetAllTicketsByTypeAsync(companyId, "Runtime")).Count,
                UIType = (await _ticketService.GetAllTicketsByTypeAsync(companyId, "UI")).Count,
                Maintenance = (await _ticketService.GetAllTicketsByTypeAsync(companyId, "Maintenance")).Count
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
