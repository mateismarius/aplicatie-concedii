using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilitar.Migrations
{
    public partial class AddLegallDays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreeDays_DayOffTypes_DayOffTypeId",
                table: "FreeDays");

            migrationBuilder.DropForeignKey(
                name: "FK_FreeDays_Employees_EmployeeId",
                table: "FreeDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FreeDays",
                table: "FreeDays");

            migrationBuilder.RenameTable(
                name: "FreeDays",
                newName: "FreeDay");

            migrationBuilder.RenameIndex(
                name: "IX_FreeDays_EmployeeId",
                table: "FreeDay",
                newName: "IX_FreeDay_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FreeDays_DayOffTypeId",
                table: "FreeDay",
                newName: "IX_FreeDay_DayOffTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreeDay",
                table: "FreeDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FreeDay_DayOffTypes_DayOffTypeId",
                table: "FreeDay",
                column: "DayOffTypeId",
                principalTable: "DayOffTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreeDay_Employees_EmployeeId",
                table: "FreeDay",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreeDay_DayOffTypes_DayOffTypeId",
                table: "FreeDay");

            migrationBuilder.DropForeignKey(
                name: "FK_FreeDay_Employees_EmployeeId",
                table: "FreeDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FreeDay",
                table: "FreeDay");

            migrationBuilder.RenameTable(
                name: "FreeDay",
                newName: "FreeDays");

            migrationBuilder.RenameIndex(
                name: "IX_FreeDay_EmployeeId",
                table: "FreeDays",
                newName: "IX_FreeDays_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FreeDay_DayOffTypeId",
                table: "FreeDays",
                newName: "IX_FreeDays_DayOffTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreeDays",
                table: "FreeDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FreeDays_DayOffTypes_DayOffTypeId",
                table: "FreeDays",
                column: "DayOffTypeId",
                principalTable: "DayOffTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreeDays_Employees_EmployeeId",
                table: "FreeDays",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
