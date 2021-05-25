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
    public class TicketAttachment
    {
        public int Id { get; set; } //PK

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        [Required]
        public string Description { get; set; }

        [DisplayName("Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public int TicketId { get; set; } //FK

        public string UserId { get; set; }
        [NotMapped]  //Prevents property from finding its way into database to a column
        [Display(Name = "Choose Ticket Image")]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; }

        public string FileName { get; set; }

        public byte[] FileData { get; set; }
        [Display(Name = "File Extension")]

        public string FileContentType { get; set; }

        //Navigational Properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }


    }
}
