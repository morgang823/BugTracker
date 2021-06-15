using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRolesService _rolesService;
        public BTProjectService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager, IBTCompanyInfoService companyInfoService, IBTRolesService rolesService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
            _rolesService = rolesService;
        }
        private async Task<Project> GetProjectByIdAsync(int projectId)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        }
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            //TODO This doesn't belong
            //if the user supplied isn't a manager, return false
            try
            {

                Project project = await GetProjectByIdAsync(projectId);
                //This returns a user who is a member, or it returns null?
                foreach (var user in project.Members)
                {
                    // If user is a project manager, return false.  
                    if (await _rolesService.IsUserInRoleAsync(user, "ProjectManager"))
                    {
                        project.Members.Remove(user);
                    }
                }
                //else, assign the new project manager to the project, then return true
                BTUser newManager = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                project.Members.Add(newManager);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //TODO Don't add them twice
        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user is not null)
                {

                    Project project = await GetProjectByIdAsync(projectId);
                    if (!await IsUserOnProjectAsync(userId, projectId))
                    {
                        try
                        {
                            project.Members.Add(user);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        catch
                        {
                            throw;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***ERROR*** - Error Adding user to project. --> {ex.Message}");
                return false;
            }
        }

        public async Task<List<BTUser>> DevelopersOnProjectAsync(int projectId)
        {
            Project project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(u => u.Id == projectId);

                List<BTUser> developers = new();
            foreach(var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user, "Developer"))
                {
                    developers.Add(user);
                }
            }
            return developers;
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            var companyProjects = (await _companyInfoService.GetAllProjectsAsync(companyId)).ToList();
            return companyProjects;

        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            var companyProjects = await _companyInfoService.GetAllProjectsAsync(companyId);
            List<Project> byPriority = new();
            foreach (var project in companyProjects)
            {
                if (project.ProjectPriority.Name == priorityName)
                {
                    byPriority.Add(project);
                }
            }
            return byPriority;
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            var companyProjects = await GetAllProjectsByCompanyAsync(companyId);
            var archivedCompanyProjects = companyProjects.Where(p => p.Archived == true).ToList();
            return archivedCompanyProjects;
        }

        public async Task<List<BTUser>> GetMembersWithoutPMAsync(int projectId)
        {
            Project project = await GetProjectByIdAsync(projectId);
            List<BTUser> nonPMUsers = new();
            foreach (var user in project.Members)
            {
                if (!await _rolesService.IsUserInRoleAsync(user, "ProjectManager"))
                {
                    nonPMUsers.Add(user);
                }
            }
            return nonPMUsers;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            //What if there isn't one?!
            // can we return a nullable? or do we return default
            BTUser projectManager = (await GetProjectMembersByRoleAsync(projectId, "ProjectManager")).FirstOrDefault();
            return projectManager;
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await GetProjectByIdAsync(projectId);
            List<BTUser> membersByRole = new();
            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role))
                {
                    membersByRole.Add(user);
                }
            }
            return membersByRole;
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await GetProjectByIdAsync(projectId);
            foreach (var user in project.Members)
            {
                if (user.Id == userId)
                {
                    return true;
                }
            }
            return false;
        }


        public async Task<List<Project>> ListUserProjectsAsync(string userId)
        {
            var projectList = (await _context.Users.Include(u => u.Projects)
                                    .ThenInclude(p => p.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                            .ThenInclude(t => t.TicketPriority)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                            .ThenInclude(t => t.Attachments)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                            .ThenInclude(t => t.History)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Tickets)
                                            .ThenInclude(t => t.Comments)
                                    .Include(u => u.Projects)
                                        .ThenInclude(p => p.Company)
                                .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();
            return projectList;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await GetProjectByIdAsync(projectId);
            BTUser projectManager = (await GetProjectMembersByRoleAsync(projectId, "ProjectManager")).FirstOrDefault();
            project.Members.Remove(projectManager);
            await _context.SaveChangesAsync();
            return;
        }
        //Should this be boolean, in case user is not on project in the first place? TODO
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Project project = await GetProjectByIdAsync(projectId);
            project.Members.Remove(user);
            await _context.SaveChangesAsync();
            return;
        }


        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            List<BTUser> usersToRemove = await GetProjectMembersByRoleAsync(projectId, role);
            Project project = await GetProjectByIdAsync(projectId);
            foreach (var user in project.Members)
            {
                if (usersToRemove.Contains(user))
                {
                    project.Members.Remove(user);
                }
            }
            await _context.SaveChangesAsync();

            return;

        }

        public Task<List<BTUser>> SubmittersOnProjectAsync(int projectId)
        {
            return GetProjectMembersByRoleAsync(projectId, "Submitter");
        }

        public async Task<List<BTUser>> UsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> allNonProjectUsers = await _context.Users.ToListAsync();
            Project project = await GetProjectByIdAsync(projectId);
            foreach (var user in project.Members)
            {
                allNonProjectUsers.Remove(user);
            }
            return allNonProjectUsers;
        }

    }
}

