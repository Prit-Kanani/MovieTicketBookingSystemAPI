using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Management_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            // Drop old FKs
            mb.DropForeignKey("FK__MovieGenr__Movie__18EBB532", "MovieGenres");
            mb.DropForeignKey("FK__MovieGenr__Genre__19DFD96B", "MovieGenres");

            // Re-add with CASCADE
            mb.AddForeignKey(
                name: "FK__MovieGenr__Movie__18EBB532",
                table: "MovieGenres",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "MovieID",
                onDelete: ReferentialAction.Cascade);

            mb.AddForeignKey(
                name: "FK__MovieGenr__Genre__19DFD96B",
                table: "MovieGenres",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "GenreID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            // Reverse to SetNull
            mb.DropForeignKey("FK__MovieGenr__Movie__18EBB532", "MovieGenres");
            mb.DropForeignKey("FK__MovieGenr__Genre__19DFD96B", "MovieGenres");

            mb.AddForeignKey(
                name: "FK__MovieGenr__Movie__18EBB532",
                table: "MovieGenres",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "MovieID",
                onDelete: ReferentialAction.SetNull);

            mb.AddForeignKey(
                name: "FK__MovieGenr__Genre__19DFD96B",
                table: "MovieGenres",
                column: "GenreID",
                principalTable: "Genres",
                principalColumn: "GenreID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
