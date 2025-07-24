using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenTOY.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    MembershipType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => new { x.Id, x.ServiceId });
                });

            migrationBuilder.CreateTable(
                name: "EmailAccounts",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Salt = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAccounts", x => new { x.ServiceId, x.Email });
                    table.ForeignKey(
                        name: "FK_EmailAccounts_Users_Id_ServiceId",
                        columns: x => new { x.Id, x.ServiceId },
                        principalTable: "Users",
                        principalColumns: new[] { "Id", "ServiceId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestAccounts",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestAccounts", x => new { x.ServiceId, x.DeviceId });
                    table.ForeignKey(
                        name: "FK_GuestAccounts_Users_Id_ServiceId",
                        columns: x => new { x.Id, x.ServiceId },
                        principalTable: "Users",
                        principalColumns: new[] { "Id", "ServiceId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailAccounts_Id_ServiceId",
                table: "EmailAccounts",
                columns: new[] { "Id", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_GuestAccounts_Id_ServiceId",
                table: "GuestAccounts",
                columns: new[] { "Id", "ServiceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAccounts");

            migrationBuilder.DropTable(
                name: "GuestAccounts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
