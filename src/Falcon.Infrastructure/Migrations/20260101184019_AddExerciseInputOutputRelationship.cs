using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseInputOutputRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId",
                table: "ExerciseOutputs",
                column: "ExerciseInputId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId",
                table: "ExerciseOutputs",
                column: "ExerciseInputId",
                principalTable: "ExerciseInputs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseOutputs_ExerciseInputs_ExerciseInputId",
                table: "ExerciseOutputs"
            );

            migrationBuilder.DropIndex(
                name: "IX_ExerciseOutputs_ExerciseInputId",
                table: "ExerciseOutputs"
            );
        }
    }
}
