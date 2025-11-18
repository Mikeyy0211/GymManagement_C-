using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym.Infrastructure.Migrations
{
    public partial class AddTrainerModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ❌ XÓA TOÀN BỘ ALTER ROWVERSION
            // ❌ XÓA ALTER MembershipPlans.RowVersion
            // ❌ XÓA ALTER Members.RowVersion
            // ❌ XÓA ALTER GymClasses.RowVersion
            // ❌ XÓA ALTER ClassSessions.RowVersion

            // Chỉ giữ lại đúng phần CREATE TABLE TrainerProfiles + FK

            migrationBuilder.CreateTable(
                name: "TrainerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerProfiles_UserId",
                table: "TrainerProfiles",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng trainer
            migrationBuilder.DropTable(
                name: "TrainerProfiles");
        }
    }
}
