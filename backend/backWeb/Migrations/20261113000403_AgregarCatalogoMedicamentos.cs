using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoMedicamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicamentos",
                columns: table => new
                {
                    IdMedicamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreComercial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PrincipioActivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Presentacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Concentracion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamentos", x => x.IdMedicamento);
                    table.CheckConstraint("CK_Medicamentos_Stock_NoNegativo", "[Stock] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "PlantillasReceta",
                columns: table => new
                {
                    IdPlantilla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePlantilla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiagnosticoAsociado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasReceta", x => x.IdPlantilla);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPlantillaReceta",
                columns: table => new
                {
                    IdDetallePlantilla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantillaRecetaId = table.Column<int>(type: "int", nullable: false),
                    MedicamentoId = table.Column<int>(type: "int", nullable: false),
                    DosisSugerida = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FrecuenciaSugerida = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DuracionSugerida = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IndicacionesSugeridas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPlantillaReceta", x => x.IdDetallePlantilla);
                    table.ForeignKey(
                        name: "FK_DetallesPlantillaReceta_Medicamentos_MedicamentoId",
                        column: x => x.MedicamentoId,
                        principalTable: "Medicamentos",
                        principalColumn: "IdMedicamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesPlantillaReceta_PlantillasReceta_PlantillaRecetaId",
                        column: x => x.PlantillaRecetaId,
                        principalTable: "PlantillasReceta",
                        principalColumn: "IdPlantilla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPlantillaReceta_MedicamentoId",
                table: "DetallesPlantillaReceta",
                column: "MedicamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPlantillaReceta_PlantillaRecetaId",
                table: "DetallesPlantillaReceta",
                column: "PlantillaRecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_NombreComercial",
                table: "Medicamentos",
                column: "NombreComercial");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasReceta_NombrePlantilla",
                table: "PlantillasReceta",
                column: "NombrePlantilla",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesPlantillaReceta");

            migrationBuilder.DropTable(
                name: "Medicamentos");

            migrationBuilder.DropTable(
                name: "PlantillasReceta");
        }
    }
}
