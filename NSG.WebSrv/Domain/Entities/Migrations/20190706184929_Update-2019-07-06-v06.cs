using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v06 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incident_NIC_NIC_Id",
                table: "Incident");

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    IncidentTypeId = table.Column<int>(nullable: false),
                    SubjectLine = table.Column<string>(maxLength: 1073741823, nullable: false),
                    EmailBody = table.Column<string>(maxLength: 1073741823, nullable: false),
                    TimeTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    ThanksTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    LogTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    Template = table.Column<string>(maxLength: 1073741823, nullable: false),
                    FromServer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => new { x.CompanyId, x.IncidentTypeId });
                    table.ForeignKey(
                        name: "FK_EmailTemplates_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_IncidentType_IncidentTypeId",
                        column: x => x.IncidentTypeId,
                        principalTable: "IncidentType",
                        principalColumn: "IncidentTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_IncidentTypeId",
                table: "EmailTemplates",
                column: "IncidentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_NIC_NIC_Id",
                table: "Incident",
                column: "NIC_Id",
                principalTable: "NIC",
                principalColumn: "NIC_Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incident_NIC_NIC_Id",
                table: "Incident");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_NIC_NIC_Id",
                table: "Incident",
                column: "NIC_Id",
                principalTable: "NIC",
                principalColumn: "NIC_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
