using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRemoteIdToTaskAndTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemoteID",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemoteID",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectID_RemoteID",
                table: "Tasks",
                columns: new[] { "ProjectID", "RemoteID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ProjectID_RemoteID",
                table: "Tags",
                columns: new[] { "ProjectID", "RemoteID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectID_RemoteID",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ProjectID_RemoteID",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "RemoteID",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RemoteID",
                table: "Tags");
        }
    }
}
