using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceTelemetryService.Migrations
{
    /// <inheritdoc />
    public partial class AddingDataIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TelemetryEvents_CustomerId_DeviceId_EventId",
                table: "TelemetryEvents",
                columns: new[] { "CustomerId", "DeviceId", "EventId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelemetryEvents_CustomerId_DeviceId_EventId",
                table: "TelemetryEvents");
        }
    }
}
