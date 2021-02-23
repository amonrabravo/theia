using Microsoft.EntityFrameworkCore.Migrations;

namespace MigrationsSqlServer.Migrations
{
    public partial class Mig003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryBanner_Banners_BannerId",
                table: "CategoryBanner");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryBanner_Categories_CategoryId",
                table: "CategoryBanner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryBanner",
                table: "CategoryBanner");

            migrationBuilder.RenameTable(
                name: "CategoryBanner",
                newName: "CategoryBanners");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryBanner_BannerId",
                table: "CategoryBanners",
                newName: "IX_CategoryBanners_BannerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryBanners",
                table: "CategoryBanners",
                columns: new[] { "CategoryId", "BannerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryBanners_Banners_BannerId",
                table: "CategoryBanners",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryBanners_Categories_CategoryId",
                table: "CategoryBanners",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryBanners_Banners_BannerId",
                table: "CategoryBanners");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryBanners_Categories_CategoryId",
                table: "CategoryBanners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryBanners",
                table: "CategoryBanners");

            migrationBuilder.RenameTable(
                name: "CategoryBanners",
                newName: "CategoryBanner");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryBanners_BannerId",
                table: "CategoryBanner",
                newName: "IX_CategoryBanner_BannerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryBanner",
                table: "CategoryBanner",
                columns: new[] { "CategoryId", "BannerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryBanner_Banners_BannerId",
                table: "CategoryBanner",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryBanner_Categories_CategoryId",
                table: "CategoryBanner",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
