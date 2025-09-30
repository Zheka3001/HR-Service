using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MiddleName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Login = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "longblob", nullable: false),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "Login", "MiddleName", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpiryTime", "Role" },
                values: new object[] { 1, "Admin", "Default", "admin", null, new byte[] { 28, 93, 86, 109, 230, 193, 118, 169, 242, 221, 102, 65, 54, 53, 133, 176, 215, 147, 87, 97, 216, 101, 127, 87, 220, 158, 144, 91, 52, 204, 131, 44, 109, 191, 253, 212, 97, 216, 128, 111, 249, 86, 210, 103, 116, 17, 172, 63, 159, 226, 202, 109, 72, 13, 220, 185, 60, 204, 122, 136, 201, 117, 225, 116 }, new byte[] { 184, 87, 74, 151, 145, 39, 13, 78, 130, 28, 212, 9, 155, 108, 28, 92, 128, 253, 141, 223, 76, 78, 28, 123, 234, 161, 106, 211, 51, 170, 78, 183, 13, 164, 200, 173, 195, 124, 91, 246, 4, 1, 229, 47, 219, 26, 212, 123, 55, 228, 223, 211, 139, 173, 68, 100, 135, 179, 201, 90, 248, 65, 41, 245, 227, 205, 41, 105, 221, 244, 80, 137, 47, 18, 214, 242, 28, 103, 58, 69, 203, 121, 127, 41, 235, 226, 28, 70, 164, 74, 9, 230, 17, 105, 68, 78, 131, 193, 169, 22, 255, 28, 89, 87, 215, 67, 192, 119, 222, 199, 124, 194, 248, 46, 112, 14, 106, 139, 197, 178, 187, 128, 127, 183, 15, 185, 212, 144 }, null, null, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
