using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompetitionGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Groups_GroupId",
                table: "Competitions"
            );

            migrationBuilder.DropIndex(name: "IX_Competitions_GroupId", table: "Competitions");

            migrationBuilder.DropColumn(name: "GroupId", table: "Competitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_GroupId",
                table: "Competitions",
                column: "GroupId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Groups_GroupId",
                table: "Competitions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id"
            );
        }
    }
}
