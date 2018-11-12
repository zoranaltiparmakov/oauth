using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TODO_WebAPI.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public virtual List<Task> Tasks { get; set; }

        public virtual List<Client> Clients { get; set; }
    }
}