using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Company Name")]

        public string Name { get; set; }

        public string Description { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]

        public IFormFile ImageFormFile { get; set; }

        [DisplayName("File Name")]

        public string ImageFileName { get; set; }

        public byte[] ImageFileData { get; set; }
        [DisplayName("File Extension")]

        public string ImageContentType { get; set; }

        //Navigational Properties
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Project> Projects {get; set; } = new HashSet<Project>();

        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();






    }
}
