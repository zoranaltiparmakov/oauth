using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TODO_WebAPI.Models;

namespace TODO_WebAPI.Controllers
{
    // authorize endpoint and allow every client to access it with every method
    [Authorize]
    public class TaskController : ApiController
    {
        [HttpGet]
        public IEnumerable<Models.Task> Get()
        {
            List<Models.Task> tasks = null;
            using (var db = new TodoAppDbContext())
            {
                tasks = db.Tasks.ToList();
            }

            return tasks;
        }

        [HttpGet]
        public Models.Task Get(int id)
        {
            Models.Task task = null;
            using (var db = new TodoAppDbContext())
            {
                task = db.Tasks.SingleOrDefault(t => t.TaskID == id);
            }

            return task;
        }

        [HttpPost]
        public void Post(Task task)
        {
            using (var db = new TodoAppDbContext())
            {
                db.Tasks.Add(new Task
                {
                    Name = task.Name,
                    Created_On = DateTime.Now,
                    User = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name)
                });

                db.SaveChanges();
            }
        }

        [HttpPut]
        public void Put(int id, Task newTask)
        {
            using (var db = new TodoAppDbContext())
            {
                var task = db.Tasks.SingleOrDefault(t => t.TaskID == id);
                task.Name = newTask.Name;
                db.SaveChanges();
            }
        }

        [HttpDelete]
        public void Delete(int id)
        {
            using (var db = new TodoAppDbContext())
            {
                var task = db.Tasks.SingleOrDefault(t => t.TaskID == id);
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
        }

        [HttpDelete]
        public void Delete()
        {
            using (var db = new TodoAppDbContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Tasks]");
                db.SaveChanges();
            }
        }

        // used for CORS pre-flight
        [HttpOptions]
        [AllowAnonymous]
        public void Options() { }
    }
}