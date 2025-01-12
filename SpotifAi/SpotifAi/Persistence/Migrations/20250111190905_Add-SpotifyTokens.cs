using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifAi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotifyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpotifyAccessTokens",
                schema: "SpotifAi",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotifyAccessTokens", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_SpotifyAccessTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SpotifAi",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpotifyAccessTokens",
                schema: "SpotifAi");
        }
    }
}
