using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Enums
{
    public enum Roles
    {
        Admin,
        [Description("Project Manager")]
        ProjectManager,
        Developer,
        Submitter,
        [Description("Demo User")]
        DemoUser,
        NewUser
    }
}
