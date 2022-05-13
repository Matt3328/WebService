using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebService.Migrations
{
    public partial class addall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Film",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Like = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Film", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoris_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FilmUserFavoris",
                columns: table => new
                {
                    FilmsLikeId = table.Column<int>(type: "int", nullable: false),
                    UserFavorisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmUserFavoris", x => new { x.FilmsLikeId, x.UserFavorisId });
                    table.ForeignKey(
                        name: "FK_FilmUserFavoris_Film_FilmsLikeId",
                        column: x => x.FilmsLikeId,
                        principalTable: "Film",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmUserFavoris_UserFavoris_UserFavorisId",
                        column: x => x.UserFavorisId,
                        principalTable: "UserFavoris",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmUserFavoris_UserFavorisId",
                table: "FilmUserFavoris",
                column: "UserFavorisId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoris_ClientId",
                table: "UserFavoris",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmUserFavoris");

            migrationBuilder.DropTable(
                name: "Film");

            migrationBuilder.DropTable(
                name: "UserFavoris");
        }
    }
}
