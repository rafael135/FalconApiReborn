using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseOutputs_Exercises_ExerciseId",
                table: "ExerciseOutputs");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisesInCompetition_Competitions_CompetitionId",
                table: "ExercisesInCompetition");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisesInCompetition_Exercises_ExerciseId",
                table: "ExercisesInCompetition");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvites_Groups_GroupId1",
                table: "GroupInvites");

            migrationBuilder.DropIndex(
                name: "IX_GroupInvites_GroupId",
                table: "GroupInvites");

            migrationBuilder.DropIndex(
                name: "IX_GroupInvites_GroupId1",
                table: "GroupInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisesInCompetition",
                table: "ExercisesInCompetition");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "GroupInvites");

            migrationBuilder.RenameTable(
                name: "ExercisesInCompetition",
                newName: "ExercisesInCompetitions");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisesInCompetition_ExerciseId",
                table: "ExercisesInCompetitions",
                newName: "IX_ExercisesInCompetitions_ExerciseId");

            migrationBuilder.AlterColumn<string>(
                name: "LeaderId",
                table: "Groups",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "Exercises",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachedFileId",
                table: "Exercises",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "ExerciseOutputs",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ExerciseInputId1",
                table: "ExerciseOutputs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "ExerciseInputs",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisesInCompetitions",
                table: "ExercisesInCompetitions",
                columns: new[] { "CompetitionId", "ExerciseId" });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionRankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Penalty = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    RankOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionRankings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionRankings_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionRankings_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupExerciseAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    JudgeResponse = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupExerciseAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupExerciseAttempts_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupExerciseAttempts_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupExerciseAttempts_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupsInCompetitions",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Blocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsInCompetitions", x => new { x.GroupId, x.CompetitionId });
                    table.ForeignKey(
                        name: "FK_GroupsInCompetitions_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsInCompetitions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    ActionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Logs_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Logs_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Questions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_GroupId_UserId",
                table: "GroupInvites",
                columns: new[] { "GroupId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_AttachedFileId",
                table: "Exercises",
                column: "AttachedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId1",
                table: "ExerciseOutputs",
                column: "ExerciseInputId1");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_GroupId",
                table: "Competitions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionRankings_CompetitionId_GroupId",
                table: "CompetitionRankings",
                columns: new[] { "CompetitionId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionRankings_CompetitionId_RankOrder",
                table: "CompetitionRankings",
                columns: new[] { "CompetitionId", "RankOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionRankings_GroupId",
                table: "CompetitionRankings",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupExerciseAttempts_CompetitionId_GroupId_ExerciseId",
                table: "GroupExerciseAttempts",
                columns: new[] { "CompetitionId", "GroupId", "ExerciseId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupExerciseAttempts_ExerciseId",
                table: "GroupExerciseAttempts",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupExerciseAttempts_GroupId",
                table: "GroupExerciseAttempts",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupExerciseAttempts_SubmissionTime",
                table: "GroupExerciseAttempts",
                column: "SubmissionTime");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsInCompetitions_CompetitionId",
                table: "GroupsInCompetitions",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActionTime",
                table: "Logs",
                column: "ActionTime");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CompetitionId",
                table: "Logs",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_GroupId",
                table: "Logs",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AnswerId",
                table: "Questions",
                column: "AnswerId",
                unique: true,
                filter: "[AnswerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CompetitionId",
                table: "Questions",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedAt",
                table: "Questions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExerciseId",
                table: "Questions",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_UserId",
                table: "Questions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Groups_GroupId",
                table: "Competitions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId1",
                table: "ExerciseOutputs",
                column: "ExerciseInputId1",
                principalTable: "ExerciseInputs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseOutputs_Exercises_ExerciseId",
                table: "ExerciseOutputs",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_AttachedFiles_AttachedFileId",
                table: "Exercises",
                column: "AttachedFileId",
                principalTable: "AttachedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisesInCompetitions_Competitions_CompetitionId",
                table: "ExercisesInCompetitions",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisesInCompetitions_Exercises_ExerciseId",
                table: "ExercisesInCompetitions",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Groups_GroupId",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId1",
                table: "ExerciseOutputs");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseOutputs_Exercises_ExerciseId",
                table: "ExerciseOutputs");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_AttachedFiles_AttachedFileId",
                table: "Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisesInCompetitions_Competitions_CompetitionId",
                table: "ExercisesInCompetitions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisesInCompetitions_Exercises_ExerciseId",
                table: "ExercisesInCompetitions");

            migrationBuilder.DropTable(
                name: "AttachedFiles");

            migrationBuilder.DropTable(
                name: "CompetitionRankings");

            migrationBuilder.DropTable(
                name: "GroupExerciseAttempts");

            migrationBuilder.DropTable(
                name: "GroupsInCompetitions");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_GroupInvites_GroupId_UserId",
                table: "GroupInvites");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_AttachedFileId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId1",
                table: "ExerciseOutputs");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_GroupId",
                table: "Competitions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisesInCompetitions",
                table: "ExercisesInCompetitions");

            migrationBuilder.DropColumn(
                name: "AttachedFileId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "ExerciseInputId1",
                table: "ExerciseOutputs");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Competitions");

            migrationBuilder.RenameTable(
                name: "ExercisesInCompetitions",
                newName: "ExercisesInCompetition");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisesInCompetitions_ExerciseId",
                table: "ExercisesInCompetition",
                newName: "IX_ExercisesInCompetition_ExerciseId");

            migrationBuilder.AlterColumn<string>(
                name: "LeaderId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId1",
                table: "GroupInvites",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "ExerciseOutputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JudgeUuid",
                table: "ExerciseInputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisesInCompetition",
                table: "ExercisesInCompetition",
                columns: new[] { "CompetitionId", "ExerciseId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_GroupId",
                table: "GroupInvites",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_GroupId1",
                table: "GroupInvites",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseOutputs_Exercises_ExerciseId",
                table: "ExerciseOutputs",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisesInCompetition_Competitions_CompetitionId",
                table: "ExercisesInCompetition",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisesInCompetition_Exercises_ExerciseId",
                table: "ExercisesInCompetition",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvites_Groups_GroupId1",
                table: "GroupInvites",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
