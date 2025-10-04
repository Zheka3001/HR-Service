using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToApplicants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "applicants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_applicants_CreatedById",
                table: "applicants",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_applicants_users_CreatedById",
                table: "applicants",
                column: "CreatedById",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicants_users_CreatedById",
                table: "applicants");

            migrationBuilder.DropIndex(
                name: "IX_applicants_CreatedById",
                table: "applicants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "applicants");
        }
    }
}
