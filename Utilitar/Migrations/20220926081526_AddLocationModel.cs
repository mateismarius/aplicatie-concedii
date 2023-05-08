using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilitar.Migrations
{
    public partial class AddLocationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegallDay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PJudecatories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PJudecatories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTribunals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    JudecatorieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTribunals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTribunals_PJudecatories_JudecatorieId",
                        column: x => x.JudecatorieId,
                        principalTable: "PJudecatories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PCurtiDeApels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    TribunalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCurtiDeApels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCurtiDeApels_PTribunals_TribunalId",
                        column: x => x.TribunalId,
                        principalTable: "PTribunals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PCurtiDeApels_TribunalId",
                table: "PCurtiDeApels",
                column: "TribunalId");

            migrationBuilder.CreateIndex(
                name: "IX_PTribunals_JudecatorieId",
                table: "PTribunals",
                column: "JudecatorieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PCurtiDeApels");

            migrationBuilder.DropTable(
                name: "PTribunals");

            migrationBuilder.DropTable(
                name: "PJudecatories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegallDay",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
