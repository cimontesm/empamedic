namespace backWeb.Models
{
    /// <summary>
    /// Un medicamento sugerido dentro de una PlantillaReceta, con su posología sugerida.
    /// Al aplicar la plantilla en una consulta, estos valores se copian y el médico puede editarlos.
    /// </summary>
    public class DetallePlantillaReceta
    {
        public int IdDetallePlantilla { get; set; }

        public int PlantillaRecetaId { get; set; }

        public PlantillaReceta Plantilla { get; set; } = null!;

        public int MedicamentoId { get; set; }

        public Medicamento Medicamento { get; set; } = null!;

        public string DosisSugerida { get; set; } = string.Empty;

        public string FrecuenciaSugerida { get; set; } = string.Empty;

        public string DuracionSugerida { get; set; } = string.Empty;

        public string IndicacionesSugeridas { get; set; } = string.Empty;

        public string ObtenerResumen()
        {
            var resumen = $"{Medicamento.NombreComercial}: {DosisSugerida}, {FrecuenciaSugerida}, por {DuracionSugerida}";
            return string.IsNullOrWhiteSpace(IndicacionesSugeridas)
                ? resumen
                : $"{resumen}. {IndicacionesSugeridas}";
        }
    }
}
