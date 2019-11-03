using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentType",
                columns: table => new
                {
                    IncidentTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IncidentTypeShortDesc = table.Column<string>(maxLength: 8, nullable: false),
                    IncidentTypeDesc = table.Column<string>(maxLength: 50, nullable: false),
                    IncidentTypeFromServer = table.Column<bool>(nullable: false),
                    IncidentTypeSubjectLine = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeEmailTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeTimeTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeThanksTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeLogTemplate = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeTemplate = table.Column<string>(maxLength: 1073741823, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentType", x => x.IncidentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "NetworkLog",
                columns: table => new
                {
                    NetworkLogId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<int>(nullable: false),
                    IncidentId = table.Column<long>(nullable: true),
                    IPAddress = table.Column<string>(maxLength: 50, nullable: false),
                    NetworkLogDate = table.Column<DateTime>(nullable: false),
                    Log = table.Column<string>(maxLength: 1073741823, nullable: false),
                    IncidentTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkLog", x => x.NetworkLogId);
                    table.ForeignKey(
                        name: "FK_NetworkLog_IncidentType_IncidentTypeId",
                        column: x => x.IncidentTypeId,
                        principalTable: "IncidentType",
                        principalColumn: "IncidentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NetworkLog_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "Idx_IncidentType_ShortDesc",
                table: "IncidentType",
                column: "IncidentTypeShortDesc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkLog_IncidentTypeId",
                table: "NetworkLog",
                column: "IncidentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkLog_ServerId",
                table: "NetworkLog",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkLog");

            migrationBuilder.DropTable(
                name: "IncidentType");
        }
    }
}
