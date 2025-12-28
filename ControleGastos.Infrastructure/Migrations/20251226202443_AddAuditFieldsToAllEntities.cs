using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGastos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Usuarios",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaAtualizacao",
                table: "Usuarios",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Transacoes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Transacoes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaAtualizacao",
                table: "Transacoes",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Pessoas",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Pessoas",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaAtualizacao",
                table: "Pessoas",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimaAtualizacao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "UltimaAtualizacao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "UltimaAtualizacao",
                table: "Pessoas");
        }
    }
}
