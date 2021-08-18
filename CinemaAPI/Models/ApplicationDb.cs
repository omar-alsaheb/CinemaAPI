using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class ApplicationDb:IdentityDbContext<ApplicationUser, ApplicationRole,string>
    {

        public ApplicationDb(DbContextOptions<ApplicationDb> options):base(options) 
        {

        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<MovieLink> MovieLink { get; set; }
        public DbSet<MovieActor> MovieActor { get; set; }
        public DbSet<Actor> Actor { get; set; }

        public DbSet<Movie> Movie { get; set; }



    }
}
