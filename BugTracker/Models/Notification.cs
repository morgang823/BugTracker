﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [DisplayName("Ticket")]

        public int TicketId { get; set; }
        [DisplayName("Subject")]

        public string Title { get; set; }

        public string Message { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Date")]

        public DateTimeOffset Created { get; set; }
        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }
        [Required]
        [DisplayName("Sender")]
        public string SenderID { get; set; }
        [DisplayName("Has Been Viewed")]

        public bool Viewed { get; set; }
        
        //Navigational properties

        public virtual Ticket Ticket { get; set; }

        public virtual BTUser Recipient { get; set; }

        public virtual BTUser Sender { get; set; }



    }
}