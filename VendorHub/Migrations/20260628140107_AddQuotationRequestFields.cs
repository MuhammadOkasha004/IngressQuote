using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorHub.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "QuotationRequests",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "QuotationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "QuotationRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "QuotationRequests");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "QuotationRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "QuotationRequests");
        }
    }
}
