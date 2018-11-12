using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TODO_WebAPI.Models
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; }

        public string Client_ID { get; set; }

        public string Client_Secret { get; set; }

        public string Redirect_URI { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created_On { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
}