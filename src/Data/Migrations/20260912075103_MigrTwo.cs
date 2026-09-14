using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongsTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersTracks");

            migrationBuilder.CreateTable(
                name: "UniqueArtists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LastReleaseDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueArtists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UniqueArtistsModelUserModel",
                columns: table => new
                {
                    FollowersId = table.Column<long>(type: "INTEGER", nullable: false),
                    TrackedArtistsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueArtistsModelUserModel", x => new { x.FollowersId, x.TrackedArtistsId });
                    table.ForeignKey(
                        name: "FK_UniqueArtistsModelUserModel_UniqueArtists_TrackedArtistsId",
                        column: x => x.TrackedArtistsId,
                        principalTable: "UniqueArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueArtistsModelUserModel_Users_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniqueArtistsModelUserModel_TrackedArtistsId",
                table: "UniqueArtistsModelUserModel",
                column: "TrackedArtistsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniqueArtistsModelUserModel");

            migrationBuilder.DropTable(
                name: "UniqueArtists");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "UsersTracks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    artists = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersTracks", x => x.Id);
                });
        }
    }
}
