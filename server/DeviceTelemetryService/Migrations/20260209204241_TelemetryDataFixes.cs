using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceTelemetryService.Migrations
{
    /// <inheritdoc />
    public partial class TelemetryDataFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelemetryEvents_CustomerId_DeviceId_EventId",
                table: "TelemetryEvents");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "TelemetryEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "TelemetryEvents");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryEvents_CustomerId_DeviceId_EventId",
                table: "TelemetryEvents",
                columns: new[] { "CustomerId", "DeviceId", "EventId" },
                unique: true);
        }
    }
}
