using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId1",
                table: "ExerciseOutputs"
            );

            migrationBuilder.DropIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId1",
                table: "ExerciseOutputs"
            );

            migrationBuilder.DropColumn(name: "ExerciseInputId1", table: "ExerciseOutputs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExerciseInputId1",
                table: "ExerciseOutputs",
                type: "uniqueidentifier",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId1",
                table: "ExerciseOutputs",
                column: "ExerciseInputId1"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId1",
                table: "ExerciseOutputs",
                column: "ExerciseInputId1",
                principalTable: "ExerciseInputs",
                principalColumn: "Id"
            );
        }
    }
}
