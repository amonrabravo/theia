using Microsoft.EntityFrameworkCore.Migrations;

namespace Theia.Migrations
{
    public partial class Mig002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_ParentId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Categories",
                type: "varchar(max)",
                unicode: false,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_ParentId",
                table: "Categories",
                columns: new[] { "Name", "ParentId" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_ParentId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_ParentId",
                table: "Categories",
                columns: new[] { "Name", "ParentId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [ParentId] IS NOT NULL");
        }
    }
}
