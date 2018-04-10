using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EFCoreUWP.Model.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Binges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HowMany = table.Column<int>(nullable: false),
                    TimeOccurred = table.Column<DateTime>(nullable: false),
                    WorthIt = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Binges", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Binges");
        }
    }
}
