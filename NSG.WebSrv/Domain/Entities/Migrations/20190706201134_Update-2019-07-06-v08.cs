using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentIncidentNotes",
                columns: table => new
                {
                    IncidentId = table.Column<long>(nullable: false),
                    IncidentNoteId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentIncidentNotes", x => new { x.IncidentId, x.IncidentNoteId });
                    table.ForeignKey(
                        name: "FK_IncidentIncidentNotes_Incident_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incident",
                        principalColumn: "IncidentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentIncidentNotes_IncidentNote_IncidentNoteId",
                        column: x => x.IncidentNoteId,
                        principalTable: "IncidentNote",
                        principalColumn: "IncidentNoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentIncidentNotes_IncidentNoteId",
                table: "IncidentIncidentNotes",
                column: "IncidentNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentIncidentNotes");
        }
    }
}
