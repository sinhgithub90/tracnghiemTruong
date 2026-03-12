using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TestLinks.Models;

namespace TestLinks.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         => optionsBuilder
             .UseLazyLoadingProxies();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TestUploadInfo>().HasKey(e => new { e.LinkId, e.StudentId });
            builder.Entity<TestUploadInfo>().HasOne(e => e.Link)
                .WithMany(e => e.UploadInfos);
            builder.Entity<TestUploadInfo>().HasMany(e => e.Files)
                .WithOne(e => e.UploadInfo);
        }

        public DbSet<UploadFile> UploadFiles { get; set; }

        public DbSet<QuizFile> QuizFiles { get; set; }

        public DbSet<QuizAnswerFile> QuizAnswerFiles { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestLinks.Models.Department> Department { get; set; }

        public DbSet<TestLinks.Models.UploadLink> UploadLinks { get; set; }

        public DbSet<TestUploadInfo> TestUploadInfos { get; set; }

    }
}
