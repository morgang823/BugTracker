//using BugTracker.Data;
//using BugTracker.Models;
//using BugTracker.Services.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BugTracker.ActionFilters
//{
//    public class CustomTicketAccess : ActionFilterAttribute
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IBTRolesService _roleService;
//        private readonly IBTTicketService _ticketService;
//        private readonly UserManager<BTUser> _userManager;

//        public CustomTicketAccess(ApplicationDbContext context, IBTRolesService roleService, IBTTicketService ticketService, UserManager<BTUser> userManager)
//        {
//            _context = context;
//            _roleService = roleService;
//            _ticketService = ticketService;
//            _userManager = userManager;
//            _httpContext = HttpContext;
//        }

//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var ticketId = Convert.ToInt32(filterContext.ActionArguments.SingleOrDefault(p => p.Value is "id"));
//            var ticket = _context.Ticket.Find(ticketId);
//            string userId = _userManager.GetUserId(_httpContext.HttpContext.User);
//            var user = _context.Users.Find(userId);
//            var myRole = _roleService.ListUserRolesAsync(user);
//        }

//    }
//}
