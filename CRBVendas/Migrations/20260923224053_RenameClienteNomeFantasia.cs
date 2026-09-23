using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRBVendas.Migrations
{
    /// <inheritdoc />
    public partial class RenameClienteNomeFantasia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome_Fantasia",
                table: "Clientes",
                newName: "NomeFantasia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeFantasia",
                table: "Clientes",
                newName: "Nome_Fantasia");
        }
    }
}
