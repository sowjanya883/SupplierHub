using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequesterUserID",
                table: "Requisitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_RequesterUserID",
                table: "Requisitions",
                column: "RequesterUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Users_RequesterUserID",
                table: "Requisitions",
                column: "RequesterUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Users_RequesterUserID",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_RequesterUserID",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RequesterUserID",
                table: "Requisitions");
        }
    }
}
