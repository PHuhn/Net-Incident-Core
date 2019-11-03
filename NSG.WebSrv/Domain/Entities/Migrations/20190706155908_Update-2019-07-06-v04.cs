using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public partial class Update20190706v04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NIC",
                columns: table => new
                {
                    NIC_Id = table.Column<string>(maxLength: 16, nullable: false),
                    NICDescription = table.Column<string>(maxLength: 255, nullable: false),
                    NICAbuseEmailAddress = table.Column<string>(maxLength: 50, nullable: true),
                    NICRestService = table.Column<string>(maxLength: 255, nullable: true),
                    NICWebSite = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NIC", x => x.NIC_Id);
                });

            migrationBuilder.CreateTable(
                name: "Incident",
                columns: table => new
                {
                    IncidentId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<int>(nullable: false),
                    IPAddress = table.Column<string>(maxLength: 50, nullable: false),
                    NIC_Id = table.Column<string>(maxLength: 16, nullable: false),
                    NetworkName = table.Column<string>(maxLength: 255, nullable: true),
                    AbuseEmailAddress = table.Column<string>(maxLength: 255, nullable: true),
                    ISPTicketNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Mailed = table.Column<bool>(nullable: false),
                    Closed = table.Column<bool>(nullable: false),
                    Special = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(maxLength: 1073741823, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incident", x => x.IncidentId);
                    table.ForeignKey(
                        name: "FK_Incident_NIC_NIC_Id",
                        column: x => x.NIC_Id,
                        principalTable: "NIC",
                        principalColumn: "NIC_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incident_NIC_Id",
                table: "Incident",
                column: "NIC_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incident");

            migrationBuilder.DropTable(
                name: "NIC");
        }
    }
}
