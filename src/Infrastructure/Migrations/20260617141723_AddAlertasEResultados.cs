using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraStellar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertasEResultados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoAlerta = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SeletorHtml = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosAlerta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConteudoExtraido = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExecutadoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    Sucesso = table.Column<bool>(type: "bit", nullable: false),
                    MensagemErro = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosAlerta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosAlerta_Alertas_AlertaId",
                        column: x => x.AlertaId,
                        principalTable: "Alertas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_UsuarioId",
                table: "Alertas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_UsuarioId_Ativo",
                table: "Alertas",
                columns: new[] { "UsuarioId", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosAlerta_AlertaId_ExecutadoEm",
                table: "ResultadosAlerta",
                columns: new[] { "AlertaId", "ExecutadoEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ResultadosAlerta");
            migrationBuilder.DropTable(name: "Alertas");
        }
    }
}
