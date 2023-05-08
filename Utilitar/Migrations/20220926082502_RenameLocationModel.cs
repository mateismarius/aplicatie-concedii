using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilitar.Migrations
{
    public partial class RenameLocationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PtId",
                table: "PTribunals",
                newName: "PjId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PjId",
                table: "PTribunals",
                newName: "PtId");
        }
    }
}
