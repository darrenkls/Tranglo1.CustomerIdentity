using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tranglo1.CustomerIdentity.IdentityServer.Infrastructure.Migrations.AuditLog
{
    public partial class AuditLogInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AuditLogId",
                incrementBy: 20,
                minValue: 1L,
                maxValue: 9223372036854775807L);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ClientAddress = table.Column<string>(type: "varchar(39)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_AuditLogs", x => x.LogId)
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropSequence(
                name: "AuditLogId");
        }
    }
}
