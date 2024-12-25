using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenTOY.Migrations
{
    /// <inheritdoc />
    public partial class EmailAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailAccounts",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Salt = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAccounts_Id_ServiceId",
                table: "EmailAccounts",
                columns: new[] { "Id", "ServiceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAccounts");
        }
    }
}
