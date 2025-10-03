using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_ApplicantInfos_ApplicantInfoId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_WorkGroups_WorkGroupId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_ApplicantInfos_ApplicantInfoId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialNetworks_ApplicantInfos_ApplicantInfoId",
                table: "SocialNetworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_WorkGroups_WorkGroupId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applicants",
                table: "Applicants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkGroups",
                table: "WorkGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SocialNetworks",
                table: "SocialNetworks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantInfos",
                table: "ApplicantInfos");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "Applicants",
                newName: "applicants");

            migrationBuilder.RenameTable(
                name: "WorkGroups",
                newName: "work_groups");

            migrationBuilder.RenameTable(
                name: "SocialNetworks",
                newName: "social_networks");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "ApplicantInfos",
                newName: "applicant_infos");

            migrationBuilder.RenameIndex(
                name: "IX_Users_WorkGroupId",
                table: "users",
                newName: "IX_users_WorkGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Login",
                table: "users",
                newName: "IX_users_Login");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_ApplicantInfoId",
                table: "employees",
                newName: "IX_employees_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicants_WorkGroupId",
                table: "applicants",
                newName: "IX_applicants_WorkGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicants_ApplicantInfoId",
                table: "applicants",
                newName: "IX_applicants_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_SocialNetworks_ApplicantInfoId",
                table: "social_networks",
                newName: "IX_social_networks_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_applicants",
                table: "applicants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_work_groups",
                table: "work_groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_social_networks",
                table: "social_networks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_applicant_infos",
                table: "applicant_infos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_applicants_applicant_infos_ApplicantInfoId",
                table: "applicants",
                column: "ApplicantInfoId",
                principalTable: "applicant_infos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_applicants_work_groups_WorkGroupId",
                table: "applicants",
                column: "WorkGroupId",
                principalTable: "work_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_applicant_infos_ApplicantInfoId",
                table: "employees",
                column: "ApplicantInfoId",
                principalTable: "applicant_infos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_social_networks_applicant_infos_ApplicantInfoId",
                table: "social_networks",
                column: "ApplicantInfoId",
                principalTable: "applicant_infos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_work_groups_WorkGroupId",
                table: "users",
                column: "WorkGroupId",
                principalTable: "work_groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicants_applicant_infos_ApplicantInfoId",
                table: "applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_applicants_work_groups_WorkGroupId",
                table: "applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_applicant_infos_ApplicantInfoId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_social_networks_applicant_infos_ApplicantInfoId",
                table: "social_networks");

            migrationBuilder.DropForeignKey(
                name: "FK_users_work_groups_WorkGroupId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applicants",
                table: "applicants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_work_groups",
                table: "work_groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_social_networks",
                table: "social_networks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applicant_infos",
                table: "applicant_infos");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "applicants",
                newName: "Applicants");

            migrationBuilder.RenameTable(
                name: "work_groups",
                newName: "WorkGroups");

            migrationBuilder.RenameTable(
                name: "social_networks",
                newName: "SocialNetworks");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "applicant_infos",
                newName: "ApplicantInfos");

            migrationBuilder.RenameIndex(
                name: "IX_users_WorkGroupId",
                table: "Users",
                newName: "IX_Users_WorkGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_users_Login",
                table: "Users",
                newName: "IX_Users_Login");

            migrationBuilder.RenameIndex(
                name: "IX_employees_ApplicantInfoId",
                table: "Employees",
                newName: "IX_Employees_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_applicants_WorkGroupId",
                table: "Applicants",
                newName: "IX_Applicants_WorkGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_applicants_ApplicantInfoId",
                table: "Applicants",
                newName: "IX_Applicants_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_social_networks_ApplicantInfoId",
                table: "SocialNetworks",
                newName: "IX_SocialNetworks_ApplicantInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applicants",
                table: "Applicants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkGroups",
                table: "WorkGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SocialNetworks",
                table: "SocialNetworks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantInfos",
                table: "ApplicantInfos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_ApplicantInfos_ApplicantInfoId",
                table: "Applicants",
                column: "ApplicantInfoId",
                principalTable: "ApplicantInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_WorkGroups_WorkGroupId",
                table: "Applicants",
                column: "WorkGroupId",
                principalTable: "WorkGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_ApplicantInfos_ApplicantInfoId",
                table: "Employees",
                column: "ApplicantInfoId",
                principalTable: "ApplicantInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialNetworks_ApplicantInfos_ApplicantInfoId",
                table: "SocialNetworks",
                column: "ApplicantInfoId",
                principalTable: "ApplicantInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_WorkGroups_WorkGroupId",
                table: "Users",
                column: "WorkGroupId",
                principalTable: "WorkGroups",
                principalColumn: "Id");
        }
    }
}
