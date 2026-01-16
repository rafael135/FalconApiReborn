using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: false
                    ),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MaxMembers = table.Column<int>(type: "int", nullable: true),
                    MaxExercises = table.Column<int>(type: "int", nullable: true),
                    MaxSubmissionSize = table.Column<int>(type: "int", nullable: true),
                    SubmissionPenalty = table.Column<TimeSpan>(type: "time", nullable: true),
                    StartInscriptions = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndInscriptions = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    StopRanking = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockSubmissions = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ExerciseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTypes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "GroupInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accepted = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),
                    GroupId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInvites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_GroupInvites_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_GroupInvites_Groups_GroupId1",
                        column: x => x.GroupId1,
                        principalTable: "Groups",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    JudgeUuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_ExerciseTypes_ExerciseTypeId",
                        column: x => x.ExerciseTypeId,
                        principalTable: "ExerciseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExerciseInputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InputContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JudgeUuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseInputs_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExercisesInCompetition",
                columns: table => new
                {
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ExercisesInCompetition",
                        x => new { x.CompetitionId, x.ExerciseId }
                    );
                    table.ForeignKey(
                        name: "FK_ExercisesInCompetition_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ExercisesInCompetition_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExerciseOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutputContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JudgeUuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseInputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId",
                        column: x => x.ExerciseInputId,
                        principalTable: "ExerciseInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ExerciseOutputs_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInputs_ExerciseId",
                table: "ExerciseInputs",
                column: "ExerciseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseOutputs_ExerciseId",
                table: "ExerciseOutputs",
                column: "ExerciseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId",
                table: "ExerciseOutputs",
                column: "ExerciseInputId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ExerciseTypeId",
                table: "Exercises",
                column: "ExerciseTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExercisesInCompetition_ExerciseId",
                table: "ExercisesInCompetition",
                column: "ExerciseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_GroupId",
                table: "GroupInvites",
                column: "GroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_GroupId1",
                table: "GroupInvites",
                column: "GroupId1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_UserId",
                table: "GroupInvites",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ExerciseOutputs");

            migrationBuilder.DropTable(name: "ExercisesInCompetition");

            migrationBuilder.DropTable(name: "GroupInvites");

            migrationBuilder.DropTable(name: "ExerciseInputs");

            migrationBuilder.DropTable(name: "Competitions");

            migrationBuilder.DropTable(name: "Exercises");

            migrationBuilder.DropTable(name: "ExerciseTypes");
        }
    }
}
