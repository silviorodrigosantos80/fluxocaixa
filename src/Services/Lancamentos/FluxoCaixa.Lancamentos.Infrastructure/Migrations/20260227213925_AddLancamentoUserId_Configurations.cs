using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxoCaixa.Lancamentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLancamentoUserId_Configurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UserId",
                table: "Lancamentos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UserId_Data",
                table: "Lancamentos",
                columns: new[] { "UserId", "Data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_UserId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_UserId_Data",
                table: "Lancamentos");
        }
    }
}
