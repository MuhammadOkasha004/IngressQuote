using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorHub.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSmtpCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SmtpAppPassword",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpEmail",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmtpAppPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SmtpEmail",
                table: "Users");
        }
    }
}
