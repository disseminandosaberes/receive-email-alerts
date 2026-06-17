using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraStellar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RedesignAlertasPublicacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alertas: remove SeletorHtml
            migrationBuilder.DropColumn(
                name: "SeletorHtml",
                table: "Alertas");

            // Alertas: adiciona campos de estado de monitoramento
            migrationBuilder.AddColumn<string>(
                name: "UltimoTituloConhecido",
                table: "Alertas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UltimaDataConhecida",
                table: "Alertas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // ResultadosAlerta: remove ConteudoExtraido
            migrationBuilder.DropColumn(
                name: "ConteudoExtraido",
                table: "ResultadosAlerta");

            // ResultadosAlerta: adiciona campos estruturados
            migrationBuilder.AddColumn<string>(
                name: "TituloPublicacao",
                table: "ResultadosAlerta",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataPublicacao",
                table: "ResultadosAlerta",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkPublicacao",
                table: "ResultadosAlerta",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NovaPublicacaoDetectada",
                table: "ResultadosAlerta",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeletorHtml",
                table: "Alertas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(name: "UltimoTituloConhecido", table: "Alertas");
            migrationBuilder.DropColumn(name: "UltimaDataConhecida", table: "Alertas");

            migrationBuilder.AddColumn<string>(
                name: "ConteudoExtraido",
                table: "ResultadosAlerta",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(name: "TituloPublicacao", table: "ResultadosAlerta");
            migrationBuilder.DropColumn(name: "DataPublicacao", table: "ResultadosAlerta");
            migrationBuilder.DropColumn(name: "LinkPublicacao", table: "ResultadosAlerta");
            migrationBuilder.DropColumn(name: "NovaPublicacaoDetectada", table: "ResultadosAlerta");
        }
    }
}
