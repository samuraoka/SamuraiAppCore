using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiAppCore.Data.Migrations
{
    public partial class AddedSamuraiBattlesToContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SamuraiBattle_Battles_BattleId",
                table: "SamuraiBattle");

            migrationBuilder.DropForeignKey(
                name: "FK_SamuraiBattle_Samurais_SamuraiId",
                table: "SamuraiBattle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SamuraiBattle",
                table: "SamuraiBattle");

            migrationBuilder.RenameTable(
                name: "SamuraiBattle",
                newName: "SamuraiBattles");

            migrationBuilder.RenameIndex(
                name: "IX_SamuraiBattle_BattleId",
                table: "SamuraiBattles",
                newName: "IX_SamuraiBattles_BattleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SamuraiBattles",
                table: "SamuraiBattles",
                columns: new[] { "SamuraiId", "BattleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SamuraiBattles_Battles_BattleId",
                table: "SamuraiBattles",
                column: "BattleId",
                principalTable: "Battles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SamuraiBattles_Samurais_SamuraiId",
                table: "SamuraiBattles",
                column: "SamuraiId",
                principalTable: "Samurais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SamuraiBattles_Battles_BattleId",
                table: "SamuraiBattles");

            migrationBuilder.DropForeignKey(
                name: "FK_SamuraiBattles_Samurais_SamuraiId",
                table: "SamuraiBattles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SamuraiBattles",
                table: "SamuraiBattles");

            migrationBuilder.RenameTable(
                name: "SamuraiBattles",
                newName: "SamuraiBattle");

            migrationBuilder.RenameIndex(
                name: "IX_SamuraiBattles_BattleId",
                table: "SamuraiBattle",
                newName: "IX_SamuraiBattle_BattleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SamuraiBattle",
                table: "SamuraiBattle",
                columns: new[] { "SamuraiId", "BattleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SamuraiBattle_Battles_BattleId",
                table: "SamuraiBattle",
                column: "BattleId",
                principalTable: "Battles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SamuraiBattle_Samurais_SamuraiId",
                table: "SamuraiBattle",
                column: "SamuraiId",
                principalTable: "Samurais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
