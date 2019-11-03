using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190708v01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "Idx_NoteType_ShortDesc",
                table: "NoteType",
                column: "NoteTypeShortDesc",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Idx_NoteType_ShortDesc",
                table: "NoteType");
        }
    }
}
