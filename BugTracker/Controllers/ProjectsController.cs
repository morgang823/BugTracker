﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using BugTracker.Extensions;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _infoService;
        private readonly UserManager<BTUser> _userManager;

        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService, IBTCompanyInfoService infoService, UserManager<BTUser> userManager)
        {
            _context = context;
            _projectService = projectService;
            _infoService = infoService;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Project.Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> CompanyProjects()
        {
            //Get CompanyID
            int companyId = User.Identity.GetCompanyId().Value;
            var tickets = (await _infoService.GetAllProjectsAsync(companyId));
            return View(tickets);
        }

        public async Task<IActionResult> MemberProjects()
        {
            //Get UserId
            var userId = _userManager.GetUserId(User);
            var projects = await _projectService.ListUserProjectsAsync(userId);
            return View(projects);
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Members)
                .Include(p => p.ProjectPriority)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate,ProjectPriorityId,ImageFileName,ImageFileData,ImageContentType,Archived")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,ProjectPriorityId,ImageFileName,ImageFileData,ImageContentType,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        //[Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]

        public async Task<IActionResult> AssignUsers(int id)
        {
            ProjectMembersViewModel model = new();

            //get company id
            int companyId = User.Identity.GetCompanyId().Value;
            Project project = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                .FirstOrDefault(project => project.Id == id);

            model.Project = project;
            List<BTUser> developers = await _infoService.GetMembersInRoleAsync(Roles.Developer.ToString(),companyId);
            List<BTUser> submitters = await _infoService.GetMembersInRoleAsync(Roles.Submitter.ToString(), companyId);

            List<BTUser> users = developers.Concat(submitters).ToList();
            List<string> members = project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);

        }

        //POST Assing Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUsers(ProjectMembersViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.SelectedUsers != null)
                {
                    List<string> memberIds = (await _projectService.GetMembersWithoutPMAsync(model.Project.Id))
                                                        .Select(m => m.Id).ToList();
                    foreach(string id in memberIds)
                    {
                        await _projectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                    }
                    foreach (string id in model.SelectedUsers)
                    {
                        await _projectService.AddUserToProjectAsync(id, model.Project.Id);
                    }
                    //goto project details
                    return RedirectToAction("Details", "Projects", new {id = model.Project.Id });
                }
                else
                {
                    //send an error message back
                }
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
