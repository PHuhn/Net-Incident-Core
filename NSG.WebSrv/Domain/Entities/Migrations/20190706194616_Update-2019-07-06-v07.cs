using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v07 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NoteType",
                columns: table => new
                {
                    NoteTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoteTypeShortDesc = table.Column<string>(maxLength: 8, nullable: false),
                    NoteTypeDesc = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteType", x => x.NoteTypeId);
                });

            migrationBuilder.CreateTable(
                name: "IncidentNote",
                columns: table => new
                {
                    IncidentNoteId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoteTypeId = table.Column<int>(nullable: false),
                    Note = table.Column<string>(maxLength: 1073741823, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentNote", x => x.IncidentNoteId);
                    table.ForeignKey(
                        name: "FK_IncidentNote_NoteType_NoteTypeId",
                        column: x => x.NoteTypeId,
                        principalTable: "NoteType",
                        principalColumn: "NoteTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incident_ServerId",
                table: "Incident",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentNote_NoteTypeId",
                table: "IncidentNote",
                column: "NoteTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_Servers_ServerId",
                table: "Incident",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incident_Servers_ServerId",
                table: "Incident");

            migrationBuilder.DropTable(
                name: "IncidentNote");

            migrationBuilder.DropTable(
                name: "NoteType");

            migrationBuilder.DropIndex(
                name: "IX_Incident_ServerId",
                table: "Incident");
        }
    }
}
