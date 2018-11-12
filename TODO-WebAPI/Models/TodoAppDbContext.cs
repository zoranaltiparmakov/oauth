using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Linq;
using System.Web;

namespace TODO_WebAPI.Models
{
    // Entity Framework class DbContext maintain connection to DB
    public class TodoAppDbContext : DbContext
    {
        public TodoAppDbContext() : base()
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}