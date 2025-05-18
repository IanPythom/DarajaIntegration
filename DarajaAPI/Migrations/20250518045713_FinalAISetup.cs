using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarajaAPI.Migrations
{
    /// <inheritdoc />
    public partial class FinalAISetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MpesaBaseUrl",
                table: "DarajaSettings",
                newName: "TransactionStatusEndpoint");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionExpiry",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Tokens",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "MpesaC2Bs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "MpesaC2Bs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "MpesaC2Bs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationResult",
                table: "MpesaC2Bs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "DarajaSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "C2BSimulateUrl",
                table: "DarajaSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InitiatorName",
                table: "DarajaSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InitiatorPassword",
                table: "DarajaSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PassKey",
                table: "DarajaSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SubscriptionExpiry",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "MpesaC2Bs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "MpesaC2Bs");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "MpesaC2Bs");

            migrationBuilder.DropColumn(
                name: "VerificationResult",
                table: "MpesaC2Bs");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "DarajaSettings");

            migrationBuilder.DropColumn(
                name: "C2BSimulateUrl",
                table: "DarajaSettings");

            migrationBuilder.DropColumn(
                name: "InitiatorName",
                table: "DarajaSettings");

            migrationBuilder.DropColumn(
                name: "InitiatorPassword",
                table: "DarajaSettings");

            migrationBuilder.DropColumn(
                name: "PassKey",
                table: "DarajaSettings");

            migrationBuilder.RenameColumn(
                name: "TransactionStatusEndpoint",
                table: "DarajaSettings",
                newName: "MpesaBaseUrl");
        }
    }
}
