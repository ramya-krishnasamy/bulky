using System;
using Bulkyweb_razor.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulkyweb_razor.Data {
	public class ApplicationDbContext: DbContext {

		public DbSet<Category> Categories { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryName = "Sci-Fi", DisplayOrder = 1 },
                new Category { Id = 2, CategoryName = "Adventure", DisplayOrder = 2 },
                new Category { Id = 3, CategoryName = "History", DisplayOrder = 3 }
                );
        }
    }
}

