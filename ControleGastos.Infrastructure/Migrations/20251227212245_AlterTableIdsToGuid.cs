using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGastos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableIdsToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Pessoas_PessoaId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Usuarios_UsuarioId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas");

            // Step 2: Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_PessoaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_UsuarioId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_UsuarioId",
                table: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias");

            // Step 3: Drop primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            // Step 4: Add temporary columns for new GUIDs
            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "NewPessoaId",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewCategoriaId",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Pessoas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Categorias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            // Step 5: Create mapping table (for preserving relationships)
            migrationBuilder.Sql(@"
                -- Map old Pessoa IDs to new GUIDs
                UPDATE t
                SET t.NewPessoaId = p.NewId
                FROM Transacoes t
                INNER JOIN Pessoas p ON t.PessoaId = p.Id;

                -- Map old Categoria IDs to new GUIDs
                UPDATE t
                SET t.NewCategoriaId = c.NewId
                FROM Transacoes t
                INNER JOIN Categorias c ON t.CategoriaId = c.Id;
            ");

            // Step 6: Drop old columns
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "PessoaId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categorias");

            // Step 7: Rename new columns to original names
            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Transacoes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NewPessoaId",
                table: "Transacoes",
                newName: "PessoaId");

            migrationBuilder.RenameColumn(
                name: "NewCategoriaId",
                table: "Transacoes",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Pessoas",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Categorias",
                newName: "Id");

            // Step 8: Make FK columns non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "PessoaId",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaId",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // Step 9: Recreate primary keys
            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            // Step 10: Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_PessoaId",
                table: "Transacoes",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_UsuarioId",
                table: "Transacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias",
                column: "UsuarioId");

            // Step 11: Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Pessoas_PessoaId",
                table: "Transacoes",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Usuarios_UsuarioId",
                table: "Transacoes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: Down migration will result in data loss as we cannot reliably convert GUIDs back to sequential ints
            throw new NotSupportedException("Cannot downgrade from GUID to INT identity columns without data loss. Please restore from backup.");
        }
    }
}
