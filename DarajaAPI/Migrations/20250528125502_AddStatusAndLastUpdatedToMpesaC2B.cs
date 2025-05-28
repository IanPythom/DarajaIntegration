using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarajaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndLastUpdatedToMpesaC2B : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "MpesaC2Bs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MpesaC2Bs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "MpesaC2Bs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MpesaC2Bs");
        }
    }
}
