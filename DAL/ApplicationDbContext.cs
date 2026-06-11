using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManageApp.Models;

namespace TaskManageApp.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(user => user.UserName).HasColumnName("Username");
                entity.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(user => user.LastName).HasMaxLength(100).IsRequired();
                entity.Property(user => user.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.TaskItem)
                .HasForeignKey(c => c.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.TaskHistories)
                .WithOne(h => h.TaskItem)
                .HasForeignKey(h => h.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data from mock repository for consistency
            var seedDate = new DateTime(2025, 11, 6, 12, 0, 0);

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "ana.kovacic",
                    NormalizedUserName = "ANA.KOVACIC",
                    Email = "ana.kovacic@example.com",
                    NormalizedEmail = "ANA.KOVACIC@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "mock-hash-1",
                    SecurityStamp = "d0e2b7f4-6fbe-4b7f-8f1e-5f8d1e0d1111",
                    ConcurrencyStamp = "e8aaf9b1-0f6e-4f4f-b1f4-7b1e11111111",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FirstName = "Ana",
                    LastName = "Kovacic",
                    CreatedAt = seedDate.AddMonths(-6)
                },
                new User
                {
                    Id = 2,
                    UserName = "marko.horvat",
                    NormalizedUserName = "MARKO.HORVAT",
                    Email = "marko.horvat@example.com",
                    NormalizedEmail = "MARKO.HORVAT@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "mock-hash-2",
                    SecurityStamp = "d8a5c7d3-1d2a-4b0f-8c5a-2f8d1e0d2222",
                    ConcurrencyStamp = "6f2fd0de-1db2-4c3c-b3f4-7b1e22222222",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FirstName = "Marko",
                    LastName = "Horvat",
                    CreatedAt = seedDate.AddMonths(-4)
                },
                new User
                {
                    Id = 3,
                    UserName = "petra.babic",
                    NormalizedUserName = "PETRA.BABIC",
                    Email = "petra.babic@example.com",
                    NormalizedEmail = "PETRA.BABIC@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "mock-hash-3",
                    SecurityStamp = "6d1c0e2f-0f0f-4d0a-8b5d-3f8d1e0d3333",
                    ConcurrencyStamp = "8f0d2d7d-7d44-4f4a-a3f4-7b1e33333333",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FirstName = "Petra",
                    LastName = "Babic",
                    CreatedAt = seedDate.AddMonths(-2)
                }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Planning", Description = "Planning and documentation tasks", Color = "#3b82f6", CreatedDate = seedDate.AddMonths(-8), IsActive = true },
                new Category { Id = 2, Name = "Development", Description = "Implementation and code review work", Color = "#10b981", CreatedDate = seedDate.AddMonths(-8), IsActive = true },
                new Category { Id = 3, Name = "Operations", Description = "Infrastructure and monitoring", Color = "#f59e0b", CreatedDate = seedDate.AddMonths(-8), IsActive = true }
            );

            // Seed Tasks
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem { Id = 1, Title = "Complete project proposal", Description = "Write and finalize the comprehensive project proposal document including scope, timeline, and budget estimates.", CreatedDate = seedDate.AddDays(-15), DueDate = seedDate.AddDays(-2), IsCompleted = false, PriorityId = (int)Priority.High, UserId = 1, CategoryId = 1 },
                new TaskItem { Id = 2, Title = "Review code changes", Description = "Review the latest code changes in the main repository branch.", CreatedDate = seedDate.AddDays(-5), DueDate = seedDate.AddDays(2), IsCompleted = false, PriorityId = (int)Priority.High, UserId = 2, CategoryId = 2 },
                new TaskItem { Id = 3, Title = "Update documentation", Description = "Update the user manual with new features released in v2.1.", CreatedDate = seedDate.AddDays(-10), DueDate = seedDate.AddDays(-1), IsCompleted = true, PriorityId = (int)Priority.Medium, UserId = 1, CategoryId = 1 },
                new TaskItem { Id = 4, Title = "Refactor authentication module", Description = "Refactor the authentication module to improve security and performance.", CreatedDate = seedDate.AddDays(-3), DueDate = seedDate.AddDays(7), IsCompleted = false, PriorityId = (int)Priority.Medium, UserId = 1, CategoryId = 2 },
                new TaskItem { Id = 5, Title = "Setup monitoring dashboard", Description = "Setup application monitoring and performance dashboard.", CreatedDate = seedDate.AddDays(-1), DueDate = seedDate.AddDays(30), IsCompleted = false, PriorityId = (int)Priority.Low, UserId = 2, CategoryId = 3 },
                new TaskItem { Id = 6, Title = "Fix login bug", Description = "Fix the issue where users cannot login with special characters in password.", CreatedDate = seedDate.AddDays(-7), DueDate = seedDate.AddDays(-1), IsCompleted = true, PriorityId = (int)Priority.High, UserId = 2, CategoryId = 2 },
                new TaskItem { Id = 7, Title = "Implement comprehensive API rate limiting and caching strategy to prevent abuse while maintaining optimal performance", Description = "Design and implement rate limiting and caching mechanisms.", CreatedDate = seedDate.AddDays(-2), DueDate = seedDate.AddDays(14), IsCompleted = false, PriorityId = (int)Priority.Medium, UserId = 1, CategoryId = 2 }
            );

            // Seed Comments
            modelBuilder.Entity<Comment>().HasData(
                new Comment { Id = 1, Content = "Proposal draft looks good. Please expand the risk section.", CreatedDate = seedDate.AddDays(-6), IsEdited = false, TaskItemId = 1, UserId = 2 },
                new Comment { Id = 2, Content = "I pushed updates to the timeline and dependency chart.", CreatedDate = seedDate.AddDays(-4), IsEdited = false, TaskItemId = 1, UserId = 1 },
                new Comment { Id = 3, Content = "Please verify null-handling in authentication middleware.", CreatedDate = seedDate.AddDays(-2), IsEdited = true, TaskItemId = 4, UserId = 3 },
                new Comment { Id = 4, Content = "Monitoring panel draft is ready for review.", CreatedDate = seedDate.AddHours(-20), IsEdited = false, TaskItemId = 5, UserId = 2 },
                new Comment { Id = 5, Content = "Rate limiting thresholds need product confirmation.", CreatedDate = seedDate.AddHours(-8), IsEdited = false, TaskItemId = 7, UserId = 1 }
            );

            // Seed Task Attachments
            modelBuilder.Entity<TaskAttachment>().HasData(
                new TaskAttachment { Id = 1, FileName = "project-proposal-v2.pdf", FilePath = "/mock-files/project-proposal-v2.pdf", TaskItemId = 1 },
                new TaskAttachment { Id = 2, FileName = "code-review-checklist.docx", FilePath = "/mock-files/code-review-checklist.docx", TaskItemId = 2 },
                new TaskAttachment { Id = 3, FileName = "monitoring-dashboard-sketch.png", FilePath = "/mock-files/monitoring-dashboard-sketch.png", TaskItemId = 5 }
            );

            // Seed Task Histories
            modelBuilder.Entity<TaskHistory>().HasData(
                new TaskHistory { Id = 1, Action = "Task created", ActionDate = seedDate.AddDays(-15), TaskItemId = 1 },
                new TaskHistory { Id = 2, Action = "Priority changed to High", ActionDate = seedDate.AddDays(-8), TaskItemId = 1 },
                new TaskHistory { Id = 3, Action = "Assigned to Marko Horvat", ActionDate = seedDate.AddDays(-5), TaskItemId = 2 },
                new TaskHistory { Id = 4, Action = "Status changed to Completed", ActionDate = seedDate.AddDays(-1), TaskItemId = 3 },
                new TaskHistory { Id = 5, Action = "Description updated", ActionDate = seedDate.AddHours(-12), TaskItemId = 7 }
            );
        }
    }
}
