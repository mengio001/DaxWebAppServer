using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizTowerPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialQuizTowerApiApplicationDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "QuizTowerPlatform");

            migrationBuilder.CreateTable(
                name: "Achievement",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    QuizLogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalAchievementPoints",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalQuizPoints = table.Column<int>(type: "int", nullable: false),
                    TotalAchievedPoints = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalAchievementPoints", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievement",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    AchievedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAchievement_Achievement_AchievementId",
                        column: x => x.AchievementId,
                        principalSchema: "QuizTowerPlatform",
                        principalTable: "Achievement",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThirdOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FourthOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswerPoints = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Question_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "QuizTowerPlatform",
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResult",
                schema: "QuizTowerPlatform",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGEDBY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    UsersCorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    UsersWrongAnswers = table.Column<int>(type: "int", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserResult_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "QuizTowerPlatform",
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuizId",
                schema: "QuizTowerPlatform",
                table: "Question",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievement_AchievementId",
                schema: "QuizTowerPlatform",
                table: "UserAchievement",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResult_QuizId",
                schema: "QuizTowerPlatform",
                table: "UserResult",
                column: "QuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question",
                schema: "QuizTowerPlatform");

            migrationBuilder.DropTable(
                name: "TotalAchievementPoints",
                schema: "QuizTowerPlatform");

            migrationBuilder.DropTable(
                name: "UserAchievement",
                schema: "QuizTowerPlatform");

            migrationBuilder.DropTable(
                name: "UserResult",
                schema: "QuizTowerPlatform");

            migrationBuilder.DropTable(
                name: "Achievement",
                schema: "QuizTowerPlatform");

            migrationBuilder.DropTable(
                name: "Quiz",
                schema: "QuizTowerPlatform");
        }
    }
}
