using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifAi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEndpointDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndpointDetails",
                schema: "SpotifAi",
                columns: table => new
                {
                    EndpointPath = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointDetails", x => x.EndpointPath);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndpointDetails",
                schema: "SpotifAi");
        }
    }
}
