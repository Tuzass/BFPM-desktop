using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniqueNamesForRemoteProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_OwnerID_Name",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_RemoteID_AssignedUserID",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerID_Name",
                table: "Projects",
                columns: new[] { "OwnerID", "Name" },
                unique: true,
                filter: "RemoteID IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_RemoteID_AssignedUserID",
                table: "Projects",
                columns: new[] { "RemoteID", "AssignedUserID" },
                unique: true,
                filter: "RemoteID IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_OwnerID_Name",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_RemoteID_AssignedUserID",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerID_Name",
                table: "Projects",
                columns: new[] { "OwnerID", "Name" },
                unique: true,
                filter: "RemoteID IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_RemoteID_AssignedUserID",
                table: "Projects",
                columns: new[] { "RemoteID", "AssignedUserID" },
                unique: true);
        }
    }
}
