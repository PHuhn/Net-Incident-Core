using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update201909061300 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteTypeClientScript",
                table: "NoteType",
                maxLength: 12,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteTypeClientScript",
                table: "NoteType");
        }
    }
}
