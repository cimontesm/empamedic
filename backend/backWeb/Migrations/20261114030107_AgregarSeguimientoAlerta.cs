using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSeguimientoAlerta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeguimientoAlertas",
                columns: table => new
                {
                    IdAlerta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdColaborador = table.Column<int>(type: "int", nullable: false),
                    IdMedicoResponsable = table.Column<int>(type: "int", nullable: false),
                    IdConsultaMedica = table.Column<int>(type: "int", nullable: true),
                    FechaControl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservacionResolucion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaNotificacionPush = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguimientoAlertas", x => x.IdAlerta);
                    table.ForeignKey(
                        name: "FK_SeguimientoAlertas_Usuarios_IdMedicoResponsable",
                        column: x => x.IdMedicoResponsable,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoAlertas_IdColaborador",
                table: "SeguimientoAlertas",
                column: "IdColaborador");

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoAlertas_IdMedicoResponsable_Estado_FechaControl",
                table: "SeguimientoAlertas",
                columns: new[] { "IdMedicoResponsable", "Estado", "FechaControl" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeguimientoAlertas");
        }
    }
}
