using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManageApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedDate", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "#3b82f6", new DateTime(2025, 3, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Planning and documentation tasks", true, "Planning" },
                    { 2, "#10b981", new DateTime(2025, 3, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Implementation and code review work", true, "Development" },
                    { 3, "#f59e0b", new DateTime(2025, 3, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Infrastructure and monitoring", true, "Operations" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "ana.kovacic@example.com", "Ana", "Kovacic", "mock-hash-1", "ana.kovacic" },
                    { 2, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "marko.horvat@example.com", "Marko", "Horvat", "mock-hash-2", "marko.horvat" },
                    { 3, new DateTime(2025, 9, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "petra.babic@example.com", "Petra", "Babic", "mock-hash-3", "petra.babic" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "DueDate", "IsCompleted", "PriorityId", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), "Write and finalize the comprehensive project proposal document including scope, timeline, and budget estimates.", new DateTime(2025, 11, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Complete project proposal", 1 },
                    { 2, 2, new DateTime(2025, 11, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Review the latest code changes in the main repository branch.", new DateTime(2025, 11, 8, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Review code changes", 2 },
                    { 3, 1, new DateTime(2025, 10, 27, 12, 0, 0, 0, DateTimeKind.Unspecified), "Update the user manual with new features released in v2.1.", new DateTime(2025, 11, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), true, 1, "Update documentation", 1 },
                    { 4, 2, new DateTime(2025, 11, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "Refactor the authentication module to improve security and performance.", new DateTime(2025, 11, 13, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Refactor authentication module", 1 },
                    { 5, 3, new DateTime(2025, 11, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), "Setup application monitoring and performance dashboard.", new DateTime(2025, 12, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 0, "Setup monitoring dashboard", 2 },
                    { 6, 2, new DateTime(2025, 10, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), "Fix the issue where users cannot login with special characters in password.", new DateTime(2025, 11, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Fix login bug", 2 },
                    { 7, 2, new DateTime(2025, 11, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), "Design and implement rate limiting and caching mechanisms.", new DateTime(2025, 11, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Implement comprehensive API rate limiting and caching strategy to prevent abuse while maintaining optimal performance", 1 }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedDate", "IsEdited", "TaskItemId", "UserId" },
                values: new object[,]
                {
                    { 1, "Proposal draft looks good. Please expand the risk section.", new DateTime(2025, 10, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 2 },
                    { 2, "I pushed updates to the timeline and dependency chart.", new DateTime(2025, 11, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 1 },
                    { 3, "Please verify null-handling in authentication middleware.", new DateTime(2025, 11, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), true, 4, 3 },
                    { 4, "Monitoring panel draft is ready for review.", new DateTime(2025, 11, 5, 16, 0, 0, 0, DateTimeKind.Unspecified), false, 5, 2 },
                    { 5, "Rate limiting thresholds need product confirmation.", new DateTime(2025, 11, 6, 4, 0, 0, 0, DateTimeKind.Unspecified), false, 7, 1 }
                });

            migrationBuilder.InsertData(
                table: "TaskAttachments",
                columns: new[] { "Id", "FileName", "FilePath", "TaskItemId" },
                values: new object[,]
                {
                    { 1, "project-proposal-v2.pdf", "/mock-files/project-proposal-v2.pdf", 1 },
                    { 2, "code-review-checklist.docx", "/mock-files/code-review-checklist.docx", 2 },
                    { 3, "monitoring-dashboard-sketch.png", "/mock-files/monitoring-dashboard-sketch.png", 5 }
                });

            migrationBuilder.InsertData(
                table: "TaskHistories",
                columns: new[] { "Id", "Action", "ActionDate", "TaskItemId" },
                values: new object[,]
                {
                    { 1, "Task created", new DateTime(2025, 10, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Priority changed to High", new DateTime(2025, 10, 29, 12, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "Assigned to Marko Horvat", new DateTime(2025, 11, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, "Status changed to Completed", new DateTime(2025, 11, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 5, "Description updated", new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaskAttachments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskAttachments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskAttachments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskHistories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskHistories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskHistories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskHistories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaskHistories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
