using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ViewModels
{
    public class MyProjectsViewModel
    {
        public IEnumerable<Project> CreatedProjects { get; set; }

        public IEnumerable<Project> MemberProjects { get; set; }

    }
}
