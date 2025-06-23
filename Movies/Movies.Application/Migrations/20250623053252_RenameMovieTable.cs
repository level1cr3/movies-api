using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Application.Migrations
{
    /// <inheritdoc />
    public partial class RenameMovieTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_movie",
                table: "movie");

            migrationBuilder.RenameTable(
                name: "movie",
                newName: "movies");

            migrationBuilder.AddPrimaryKey(
                name: "pk_movies",
                table: "movies",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_movies",
                table: "movies");

            migrationBuilder.RenameTable(
                name: "movies",
                newName: "movie");

            migrationBuilder.AddPrimaryKey(
                name: "pk_movie",
                table: "movie",
                column: "id");
        }
    }
}
