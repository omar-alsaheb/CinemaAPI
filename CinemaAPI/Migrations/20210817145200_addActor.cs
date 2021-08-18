using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaAPI.Migrations
{
    public partial class addActor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorName",
                table: "MovieActor");

            migrationBuilder.DropColumn(
                name: "ActorPicture",
                table: "MovieActor");

            migrationBuilder.AddColumn<int>(
                name: "ActorId",
                table: "MovieActor",
                maxLength: 150,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Actor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorName = table.Column<string>(maxLength: 150, nullable: false),
                    ActorPicture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieActor_ActorId",
                table: "MovieActor",
                column: "ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActor_Actor_ActorId",
                table: "MovieActor",
                column: "ActorId",
                principalTable: "Actor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieActor_Actor_ActorId",
                table: "MovieActor");

            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DropIndex(
                name: "IX_MovieActor_ActorId",
                table: "MovieActor");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "MovieActor");

            migrationBuilder.AddColumn<string>(
                name: "ActorName",
                table: "MovieActor",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActorPicture",
                table: "MovieActor",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
