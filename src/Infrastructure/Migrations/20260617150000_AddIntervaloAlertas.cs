using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraStellar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIntervaloAlertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntervaloMinutos",
                table: "Alertas",
                type: "int",
                nullable: false,
                defaultValue: 60);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UltimaExecucaoEm",
                table: "Alertas",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IntervaloMinutos", table: "Alertas");
            migrationBuilder.DropColumn(name: "UltimaExecucaoEm", table: "Alertas");
        }
    }
}
