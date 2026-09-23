using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRBVendas.Migrations
{
    /// <inheritdoc />
    public partial class CNPJ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CNPJ",
                table: "Clientes",
                type: "varchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InscricaoEstadual",
                table: "Clientes",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CNPJ",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "InscricaoEstadual",
                table: "Clientes");
        }
    }
}
