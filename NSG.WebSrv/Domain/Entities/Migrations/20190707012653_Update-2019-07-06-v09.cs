using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v09 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Log");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Companies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "Idx_Companies_ShortName",
                table: "Companies",
                column: "CompanyShortName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Idx_Companies_ShortName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "Log",
                nullable: false,
                defaultValue: 0);
        }
    }
}
