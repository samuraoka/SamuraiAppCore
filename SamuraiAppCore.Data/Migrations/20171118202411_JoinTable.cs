using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiAppCore.Data.Migrations
{
    public partial class JoinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samurais_Battles_BattleId",
                table: "Samurais");

            migrationBuilder.DropIndex(
                name: "IX_Samurais_BattleId",
                table: "Samurais");

            migrationBuilder.DropColumn(
                name: "BattleId",
                table: "Samurais");

            migrationBuilder.CreateTable(
                name: "SamuraiBattle",
                columns: table => new
                {
                    SamuraiId = table.Column<int>(type: "int", nullable: false),
                    BattleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamuraiBattle", x => new { x.SamuraiId, x.BattleId });
                    table.ForeignKey(
                        name: "FK_SamuraiBattle_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SamuraiBattle_Samurais_SamuraiId",
                        column: x => x.SamuraiId,
                        principalTable: "Samurais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SamuraiBattle_BattleId",
                table: "SamuraiBattle",
                column: "BattleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SamuraiBattle");

            migrationBuilder.AddColumn<int>(
                name: "BattleId",
                table: "Samurais",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Samurais_BattleId",
                table: "Samurais",
                column: "BattleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samurais_Battles_BattleId",
                table: "Samurais",
                column: "BattleId",
                principalTable: "Battles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
