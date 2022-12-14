using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace site101.Models
{
    public class ApplicationDBContext : DbContext
    { 
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options): base(options) 
        {
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
