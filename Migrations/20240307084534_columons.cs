using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNB_challenge_backend.Migrations
{
    /// <inheritdoc />
    public partial class columons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "TokenSupplies",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TokenSupplies",
                newName: "Date");
        }
    }
}
