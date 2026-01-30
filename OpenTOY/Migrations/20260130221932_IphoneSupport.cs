using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenTOY.Migrations
{
    /// <inheritdoc />
    public partial class IphoneSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "GuestAccounts",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "GuestAccounts",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);
        }
    }
}
