using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserIdToChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Changes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Changes_UserID_TargetType_TargetID",
                table: "Changes",
                columns: new[] { "UserID", "TargetType", "TargetID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_Users_UserID",
                table: "Changes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Changes_Users_UserID",
                table: "Changes");

            migrationBuilder.DropIndex(
                name: "IX_Changes_UserID_TargetType_TargetID",
                table: "Changes");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Changes");
        }
    }
}
