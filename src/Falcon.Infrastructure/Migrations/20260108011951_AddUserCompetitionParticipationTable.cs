using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCompetitionParticipationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCompetitionParticipations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompetitionParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompetitionParticipations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_UserCompetitionParticipations_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_UserCompetitionParticipations_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionParticipations_CompetitionId_UserId",
                table: "UserCompetitionParticipations",
                columns: new[] { "CompetitionId", "UserId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionParticipations_GroupId",
                table: "UserCompetitionParticipations",
                column: "GroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionParticipations_UserId",
                table: "UserCompetitionParticipations",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserCompetitionParticipations");
        }
    }
}
