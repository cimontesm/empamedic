namespace backWeb.Models
{
    /// <summary>
    /// Medicamento del catálogo del dispensario (diagrama de clases: Medicamento).
    /// </summary>
    public class Medicamento
    {
        public int IdMedicamento { get; set; }

        public string NombreComercial { get; set; } = string.Empty;

        public string PrincipioActivo { get; set; } = string.Empty;

        public string Presentacion { get; set; } = string.Empty;

        public string Concentracion { get; set; } = string.Empty;

        public List<DetallePlantillaReceta> DetallesPlantilla { get; set; } = new();

        public string ObtenerDescripcionCompleta()
        {
            return $"{NombreComercial} ({PrincipioActivo}) {Concentracion} - {Presentacion}";
        }
    }
}
