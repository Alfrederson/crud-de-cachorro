using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace escola_dos_catioros.Migrations
{
    /// <inheritdoc />
    public partial class unique_matricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matriculas_TurmaId",
                table: "Matriculas");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_TurmaId_CatioroId",
                table: "Matriculas",
                columns: new[] { "TurmaId", "CatioroId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matriculas_TurmaId_CatioroId",
                table: "Matriculas");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_TurmaId",
                table: "Matriculas",
                column: "TurmaId");
        }
    }
}
