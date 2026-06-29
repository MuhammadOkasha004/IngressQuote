using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorHub.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationAndAssignmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "VendorResponses");

            migrationBuilder.DropColumn(
                name: "InvitationToken",
                table: "QuotationAssignments");

            migrationBuilder.DropColumn(
                name: "TokenExpiry",
                table: "QuotationAssignments");

            migrationBuilder.RenameColumn(
                name: "DeliveryTimeDays",
                table: "VendorResponses",
                newName: "QuotationRequestId");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "VendorInvitations",
                newName: "ExpiresAt");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryDays",
                table: "VendorResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VendorInvitations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QuotationRequestId",
                table: "VendorInvitations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VendorName",
                table: "VendorInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "QuotationAssignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VendorResponses_QuotationAssignmentId",
                table: "VendorResponses",
                column: "QuotationAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorResponses_QuotationRequestId",
                table: "VendorResponses",
                column: "QuotationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorResponses_VendorId",
                table: "VendorResponses",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationAssignments_QuotationRequestId",
                table: "QuotationAssignments",
                column: "QuotationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationAssignments_VendorId",
                table: "QuotationAssignments",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationAssignments_QuotationRequests_QuotationRequestId",
                table: "QuotationAssignments",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "QuotationRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationAssignments_Vendors_VendorId",
                table: "QuotationAssignments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorResponses_QuotationAssignments_QuotationAssignmentId",
                table: "VendorResponses",
                column: "QuotationAssignmentId",
                principalTable: "QuotationAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorResponses_QuotationRequests_QuotationRequestId",
                table: "VendorResponses",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "QuotationRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorResponses_Vendors_VendorId",
                table: "VendorResponses",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationAssignments_QuotationRequests_QuotationRequestId",
                table: "QuotationAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationAssignments_Vendors_VendorId",
                table: "QuotationAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorResponses_QuotationAssignments_QuotationAssignmentId",
                table: "VendorResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorResponses_QuotationRequests_QuotationRequestId",
                table: "VendorResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorResponses_Vendors_VendorId",
                table: "VendorResponses");

            migrationBuilder.DropIndex(
                name: "IX_VendorResponses_QuotationAssignmentId",
                table: "VendorResponses");

            migrationBuilder.DropIndex(
                name: "IX_VendorResponses_QuotationRequestId",
                table: "VendorResponses");

            migrationBuilder.DropIndex(
                name: "IX_VendorResponses_VendorId",
                table: "VendorResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuotationAssignments_QuotationRequestId",
                table: "QuotationAssignments");

            migrationBuilder.DropIndex(
                name: "IX_QuotationAssignments_VendorId",
                table: "QuotationAssignments");

            migrationBuilder.DropColumn(
                name: "DeliveryDays",
                table: "VendorResponses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VendorInvitations");

            migrationBuilder.DropColumn(
                name: "QuotationRequestId",
                table: "VendorInvitations");

            migrationBuilder.DropColumn(
                name: "VendorName",
                table: "VendorInvitations");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "QuotationAssignments");

            migrationBuilder.RenameColumn(
                name: "QuotationRequestId",
                table: "VendorResponses",
                newName: "DeliveryTimeDays");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "VendorInvitations",
                newName: "ExpiryDate");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "VendorResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationToken",
                table: "QuotationAssignments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiry",
                table: "QuotationAssignments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
