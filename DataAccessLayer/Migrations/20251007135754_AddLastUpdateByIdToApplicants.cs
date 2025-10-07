using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdateByIdToApplicants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "applicants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_applicants_LastUpdatedById",
                table: "applicants",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_applicants_users_LastUpdatedById",
                table: "applicants",
                column: "LastUpdatedById",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicants_users_LastUpdatedById",
                table: "applicants");

            migrationBuilder.DropIndex(
                name: "IX_applicants_LastUpdatedById",
                table: "applicants");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "applicants");
        }
    }
}
