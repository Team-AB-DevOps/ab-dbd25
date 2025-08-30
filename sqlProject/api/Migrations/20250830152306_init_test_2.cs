using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class init_test_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeUser_privileges_PrivilegesId",
                table: "PrivilegeUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeUser_users_UsersId",
                table: "PrivilegeUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeUser",
                table: "PrivilegeUser");

            migrationBuilder.RenameTable(
                name: "PrivilegeUser",
                newName: "users_privileges");

            migrationBuilder.RenameIndex(
                name: "IX_PrivilegeUser_UsersId",
                table: "users_privileges",
                newName: "IX_users_privileges_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users_privileges",
                table: "users_privileges",
                columns: new[] { "PrivilegesId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_users_privileges_privileges_PrivilegesId",
                table: "users_privileges",
                column: "PrivilegesId",
                principalTable: "privileges",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_privileges_users_UsersId",
                table: "users_privileges",
                column: "UsersId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_privileges_privileges_PrivilegesId",
                table: "users_privileges");

            migrationBuilder.DropForeignKey(
                name: "FK_users_privileges_users_UsersId",
                table: "users_privileges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users_privileges",
                table: "users_privileges");

            migrationBuilder.RenameTable(
                name: "users_privileges",
                newName: "PrivilegeUser");

            migrationBuilder.RenameIndex(
                name: "IX_users_privileges_UsersId",
                table: "PrivilegeUser",
                newName: "IX_PrivilegeUser_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeUser",
                table: "PrivilegeUser",
                columns: new[] { "PrivilegesId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeUser_privileges_PrivilegesId",
                table: "PrivilegeUser",
                column: "PrivilegesId",
                principalTable: "privileges",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeUser_users_UsersId",
                table: "PrivilegeUser",
                column: "UsersId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
