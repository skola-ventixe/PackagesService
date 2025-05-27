using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Provider.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedissuesinPackageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Benefits_PackageId",
                table: "Benefits");

            migrationBuilder.RenameColumn(
                name: "Benefit",
                table: "Benefits",
                newName: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Benefits_PackageId",
                table: "Benefits",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Benefits_PackageId",
                table: "Benefits");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Benefits",
                newName: "Benefit");

            migrationBuilder.CreateIndex(
                name: "IX_Benefits_PackageId",
                table: "Benefits",
                column: "PackageId",
                unique: true);
        }
    }
}
